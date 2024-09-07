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
    }
}
