﻿namespace Service
{
    public class EmailTemplate
    {
        #region authenTemplate
        public string authenTemplate = "<!DOCTYPE html>" +
                    "<html lang=\"vi\">" +
                    "<head>" +
                    "<meta charset=\"UTF-8\">" +
                    "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                    "<title>Xác nhận Email</title>" +
                    "<style>" +
                    "body {" +
                    "font-family: Arial, sans-serif;" +
                    "background-color: #f4f4f4;" +
                    "margin: 0;" +
                    "padding: 0;" +
                    "color: #333;" +
                    "}" +
                    ".email-container {" +
                    "max-width: 600px;" +
                    "margin: 0 auto;" +
                    "background-color: #ffffff;" +
                    "padding: 20px;" +
                    "border-radius: 8px;" +
                    "box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);" +
                    "border: 1px solid #ddd;" +
                    "}" +
                    ".header {" +
                    "color: #ff7081;" +
                    "font-size: 24px;" +
                    "font-weight: bold;" +
                    "margin-bottom: 20px;" +
                    "}" +
                    ".content {" +
                    "font-size: 16px;" +
                    "line-height: 1.5;" +
                    "margin-bottom: 20px;" +
                    "}" +
                    ".content p {" +
                    "margin: 10px 0;" +
                    "}" +
                    ".verify-button {" +
                    "padding: 10px 20px;" +
                    "text-decoration: none;" +
                    "font-size: 16px;" +
                    "color: #ffffff !important ;" +
                    "background-color: #ff7081;" +
                    "text-decoration: none;" +
                    "border-radius: 5px;" +
                    "margin-top: 15px;" +
                    "text-align: center;" +
                    "margin-left: auto;" +
                    "margin-right: auto;" +
                    "width: 100px;" +
                    "display: block;" +
                    "}" +
                    ".verify-button:hover {" +
                    "opacity: 80;" +
                    "}" +
                    ".footer {" +
                    "margin-top: 30px;" +
                    "font-size: 14px;" +
                    "color: #888;" +
                    "border-top: 1px solid #ddd;" +
                    "padding-top: 20px;" +
                    "text-align: center;" +
                    "}" +
                    ".footer p {" +
                    "margin: 15px 0;" +
                    "}" +
                    ".footer .signature {" +
                    "text-align: left;" +
                    "margin-bottom: 20px;" +
                    "}" +
                    ".footer .copyright {" +
                    "font-size: 12px;" +
                    "color: #999;" +
                    "}" +
                    "</style>" +
                    "</head>" +
                    "<body>" +
                    "<div class=\"email-container\">" +
                    "<div class=\"header\">" +
                    "Kính chào khách hàng thân mến," +
                    "</div>" +
                    "<div class=\"content\">" +
                    "<p>Email này được gửi để xác nhận rằng chúng tôi đã nhận được yêu cầu <strong>{Action}</strong> của bạn và địa chỉ email này" +
                    " hiện đang được xác minh.</p>" +
                    "<p>Nếu bạn đã yêu cầu thay đổi hoặc cập nhật thông tin, vui lòng nhấn vào nút dưới đây để hoàn tất quá trình" +
                    " xác minh:</p>" +
                    "<a href=\"{confirmLink}\" class=\"verify-button\">Xác nhận</a>" +
                    "<p><strong>Lưu ý:</strong> Liên kết xác minh có hiệu lực trong 1 khoản thời gian nhất định.</p>" +
                    "<p>Vui lòng không gửi liên kết này cho người khác vì lý do bảo mật thông tin.</p>" +
                    "<p>Nếu bạn không yêu cầu xác nhận này, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi để được hỗ trợ." +
                    "</p>" +
                    "</div>" +
                    "<div class=\"footer\">" +
                    "<div class=\"signature\">" +
                    "<p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi.</p>" +
                    "<p>Trân trọng,<br><br>{projectName}</p>" +
                    "</div>" +
                    "<div class=\"copyright\">" +
                    "<p>© 2024 Bản quyền thuộc về {projectName}.</p>" +
                    "</div>" +
                    "</div>" +
                    "</div>" +
                    "</body>" +
                    "</html>";
        #endregion

        #region uploadDataErrorTemplate
        public string uploadDataErrorTemplate = @"
                <!DOCTYPE html>
                <html lang=""vi"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Thông Báo Lỗi Job</title>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            color: #333;
                            margin: 0;
                            padding: 20px;
                        }
                        .container {
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            overflow: hidden;
                        }
                        .header {
                            background-color: #ff7081;
                            padding: 20px;
                            text-align: center;
                            color: #ffffff;
                        }
                        .content {
                            padding: 20px;
                        }
                        .footer {
                            font-size: 14px;
                            color: #888;
                            border-top: 1px solid #ddd;
                            text-align: center;
                            margin-left: 20px; /* Thêm margin-left ở đây */
                        }
                        .footer p {
                            margin: 15px 0;
                        }
                        .footer .signature {
                            text-align: left;
                            margin-bottom: 20px;
                        }
                        .footer .copyright {
                            font-size: 12px;
                            color: #999;
                        }
                        .button-container {
                            text-align: center;
                            margin: 10px 0 15px 0;
                        }
                        .button {
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #ff7081;
                            color: #ffffff !important;
                            text-decoration: none;
                            border-radius: 5px;
                        }
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h1>Thông Báo Lỗi</h1>
                        </div>
                        <div class=""content"">
                            <p>Xin chào Admin,</p>
                            <p>Job <strong>cập nhật dữ liệu hệ thống hằng ngày</strong> đã gặp lỗi trong quá trình thực thi.</p>
                            <p><strong>Thời gian:</strong> {timeHappend}</p>
                            <p>Vui lòng kiểm tra chi tiết lỗi trong <strong>Logs Database</strong> bằng cách truy cập vào link dưới đây:</p>
                            <div class=""button-container"">
                                <a href=""{logLink}"" class=""button"">Xem Logs</a>
                            </div>
                            <p>Và sử dụng câu lệnh này để truy vấn: SELECT * FROM ""logs"" WHERE ""message"" LIKE '%{keyWord}%';</p>
                        </div>
                        <div class=""footer"">
                            <div class=""signature"">
                                <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với đội ngũ kỹ thuật.</p>
                                <p>Trân trọng,<br><br>{projectName}</p>
                            </div>
                            <div class=""copyright"">
                                <p>© 2024 Bản quyền thuộc về {projectName}.</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";
        #endregion

        #region influencer offer 
        public string influencerOffer = @"
                        <!DOCTYPE html>
                        <html lang=""vi"">
                          <head>
                            <meta charset=""UTF-8"" />
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                            <title>Thông Báo Influencer Tham Gia Chiến Dịch</title>
                            <style>
                              body {
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                              }
                              .email-container {
                                background-color: white;
                                max-width: 600px;
                                margin: 0 auto;
                                border-radius: 8px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                overflow: hidden;
                              }
                              .email-header {
                                background-color: rgb(255, 112, 129);
                                color: white;
                                text-align: center;
                                padding: 25px;
                                font-size: 22px;
                                font-weight: bold;
                              }
                              .email-body {
                                padding: 20px;
                                color: #333;
                              }
                              .email-body h2 {
                                color: rgb(255, 112, 129);
                              }
                              .email-body p {
                                line-height: 1.6;
                              }
                              .email-body ul {
                                padding-left: 20px;
                              }
                              .button-container {
                                text-align: center;
                                margin: 20px 0;
                              }
                              .button {
                                background-color: rgb(255, 112, 129);
                                color: white;
                                padding: 10px 20px;
                                text-align: center;
                                text-decoration: none;
                                display: inline-block;
                                border-radius: 5px;
                                font-weight: bold;
                              }
                              .email-footer {
                                padding: 20px;
                                font-size: 12px;
                                color: #777;
                                border-top: 1px solid #eaeaea;
                              }
                              .email-footer .copyright {
                                text-align: center;
                              }
                            </style>
                          </head>
                          <body>
                            <div class=""email-container"">
                              <div class=""email-header"">Thông Báo Influencer Tham Gia Chiến Dịch</div>
                              <div class=""email-body"">
                                <h2>Xin chào {BrandName},</h2>
                                <p>
                                  Chúng tôi xin thông báo rằng influencer
                                  <strong>{InfluencerName}</strong> đã bày tỏ nguyện vọng tham gia vào
                                  chiến dịch của bạn với công việc:
                                </p>
                                <p><strong>Nền tảng: {Platform}, loại nội dung: {ContentType}, số lượng: {Quantity}.</strong></p>
                                <p>Thông tin chi tiết về công việc:</p>
                                <ul>
                                  <li>Số tiền: {JobPayment} VND</li>
                                  <li>Thời gian tạo : {CreatedAt}</li>
                                  <li>Mô tả: {JobDescription}</li>
                                </ul>
                                <p>
                                  Vui lòng xem xét và phản hồi để xác nhận sự tham gia của influencer.
                                </p>
                                <div class=""button-container"">
                                  <a href=""{JobLink}"" class=""button"">Xem Chi Tiết</a>
                                </div>
                              </div>
                              <div class=""email-footer"">
                                <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                                <p>Trân trọng,<br /><br />{projectName}</p>
                                <div class=""copyright"">
                                  <p>© 2024 Bản quyền thuộc về {projectName}.</p>
                                </div>
                              </div>
                            </div>
                          </body>
                        </html>

                        ";
        #endregion
        #region brand offer 
        public string brandOffer = @"
                            <!DOCTYPE html>
                            <html lang=""vi"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Job Offer</title>
                                <style>
                                    body {
                                        font-family: Arial, sans-serif;
                                        background-color: #f4f4f4;
                                        margin: 0;
                                        padding: 0;
                                    }
                                    .email-container {
                                        background-color: white;
                                        max-width: 600px;
                                        margin: 0 auto;
                                        border-radius: 8px;
                                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                        overflow: hidden;
                                    }
                                    .email-header {
                                        background-color: rgb(255, 112, 129);
                                        color: white;
                                        text-align: center;
                                        padding: 25px;
                                        font-size: 22px;
                                        font-weight: bold;
                                    }
                                    .email-body {
                                        padding: 20px;
                                        color: #333;
                                    }
                                    .email-body h2 {
                                        color: rgb(255, 112, 129);
                                    }
                                    .email-body p {
                                        line-height: 1.6;
                                    }
                                    .email-body ul {
                                        padding-left: 20px;
                                    }
                                    .email-footer {
                                        padding: 20px;
                                        font-size: 12px;
                                        color: #777;
                                        border-top: 1px solid #eaeaea;
                                    }
	                            .email-footer .copyright{
		                            text-align: center;
	                            }
                                    .button-container {
                                        text-align: center;
                                    }
                                    .button {
                                        background-color: rgb(255, 112, 129);
                                        color: white;
                                        padding: 10px 20px;
                                        text-align: center;
                                        text-decoration: none;
                                        display: inline-block;
                                        border-radius: 5px;
                                        font-weight: bold;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class=""email-container"">
                                    <div class=""email-header"">
                                        Thông báo Mời Tham Gia Chiến Dịch
                                    </div>
                                    <div class=""email-body"">
                                        <h2>Xin chào {InfluencerName},</h2>
                                        <p>Nhãn hàng <strong>{BrandName}</strong> mời bạn tham gia vào một chiến dịch của họ với công việc sau:</p>
                                        <p><strong>Nền tảng: {Platform}, loại nội dung: {ContentType}, số lượng: {Quantity}.</strong></p>
                                        <p>Chi tiết về công việc bao gồm:</p>
                                        <ul>
                                            <li>Số tiền : {JobPayment} VND</li>
                                            <li>Thời gian tạo : {CreatedAt}</li>
                                            <li>Mô tả: {JobDescription}</li>
                                        </ul>
                                        <p>Vui lòng nhấn vào nút bên dưới để xem thêm chi tiết và chấp nhận công việc:</p>
                                        <div class=""button-container"">
                                            <a href=""{JobLink}"" class=""button"">Xem Chi Tiết</a>
                                        </div>
                                    </div>
                                    <div class=""email-footer"">
                                        <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                                        <p>Trân trọng,<br><br>{projectName}</p>
                                         <div class=""copyright"">
		                            <p>© 2024 Bản quyền thuộc về {projectName}.</p>
	                                 </div>
                                    </div>
                                </div>
                            </body>
                            </html>     
            ";
        #endregion
    }
}
