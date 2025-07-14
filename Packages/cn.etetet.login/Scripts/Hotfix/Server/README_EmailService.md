# 邮件服务配置说明

## 概述

本项目已实现通过邮箱发送验证码的功能。邮件发送服务支持通过HTTP API调用第三方邮件服务提供商来发送邮件。

## 文件说明

- `EmailService.cs`: 邮件发送服务类，包含发送验证码邮件的核心逻辑
- `EmailConfig.cs`: 邮件服务配置类，包含所有邮件相关的配置信息
- `HttpHelper.cs`: HTTP请求辅助类，用于发送HTTP POST请求
- `C2R_VerificationCodeRequestHandler.cs`: 验证码请求处理器，已集成邮件发送功能

## 配置步骤

### 1. 选择邮件服务提供商

推荐的邮件服务提供商：

- **SendGrid**: https://sendgrid.com/
  - 免费额度：每月100封邮件
  - API文档：https://docs.sendgrid.com/api-reference/mail-send/mail-send

- **Mailgun**: https://www.mailgun.com/
  - 免费额度：每月5000封邮件
  - API文档：https://documentation.mailgun.com/en/latest/api-sending.html

- **阿里云邮件推送**: https://www.aliyun.com/product/directmail
  - 按量付费
  - API文档：https://help.aliyun.com/document_detail/29444.html

- **腾讯云邮件推送**: https://cloud.tencent.com/product/ses
  - 按量付费
  - API文档：https://cloud.tencent.com/document/product/1288/51034

### 2. 修改配置信息

在 `EmailConfig.cs` 文件中修改以下配置：

```csharp
// 邮件服务API地址 - 替换为实际的API地址
public const string EMAIL_API_URL = "https://api.sendgrid.com/v3/mail/send";

// API密钥 - 替换为实际的API密钥
public const string API_KEY = "SG.your-actual-api-key-here";

// 发件人邮箱 - 替换为已验证的发件人邮箱
public const string SENDER_EMAIL = "noreply@yourdomain.com";

// 发件人名称
public const string SENDER_NAME = "您的应用名称";
```

### 3. 安全建议

**重要**: 不要将API密钥硬编码在代码中！建议：

1. 使用环境变量存储敏感信息
2. 使用配置文件（不提交到版本控制）
3. 使用密钥管理服务

示例环境变量配置：
```csharp
public static string API_KEY => Environment.GetEnvironmentVariable("EMAIL_API_KEY") ?? "your-default-key";
```

## 使用方法

### 发送验证码邮件

```csharp
// 在处理器中调用
bool emailSent = await EmailService.SendVerificationCodeAsync("user@example.com", "123456");
if (!emailSent)
{
    // 处理发送失败的情况
    Log.Error("邮件发送失败");
}
```

### 错误处理

系统已添加 `EErrCode_Login.邮件发送失败` 错误码，用于处理邮件发送失败的情况。

## 测试

1. 确保配置了正确的邮件服务API信息
2. 启动服务器
3. 发送验证码请求
4. 检查目标邮箱是否收到验证码邮件
5. 查看服务器日志确认发送状态

## 故障排除

### 常见问题

1. **邮件发送失败**
   - 检查API密钥是否正确
   - 检查发件人邮箱是否已验证
   - 检查网络连接
   - 查看服务器日志获取详细错误信息

2. **邮件未收到**
   - 检查垃圾邮件文件夹
   - 确认收件人邮箱地址正确
   - 检查邮件服务商的发送限制

3. **API调用失败**
   - 检查API地址是否正确
   - 检查请求格式是否符合服务商要求
   - 检查API配额是否用完

### 日志查看

邮件发送的详细日志会输出到服务器日志中，包括：
- 发送成功/失败状态
- API响应信息
- 错误详情

## 扩展功能

可以根据需要扩展以下功能：

1. **邮件模板系统**: 支持多种邮件模板
2. **多语言支持**: 根据用户语言发送不同语言的邮件
3. **邮件队列**: 使用队列系统处理大量邮件发送
4. **发送统计**: 记录邮件发送成功率和失败原因
5. **重试机制**: 发送失败时自动重试

## 注意事项

1. 遵守邮件服务商的使用条款和发送限制
2. 实施适当的频率限制，避免被标记为垃圾邮件
3. 定期监控邮件发送状态和成功率
4. 保护用户隐私，不要记录敏感的邮件内容