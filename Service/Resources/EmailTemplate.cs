﻿namespace Service
{
    public class EmailTemplate
    {
        #region authenTemplate
        public string authenTemplate = @"<!DOCTYPE html>
                        <html lang=""vi"">
                        <head>
                            <meta charset=""UTF-8"">
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                            <title>Xác nhận Email</title>
                            <style>
                                body {
                                    font-family: Arial, sans-serif;
                                    background-color: #f4f4f4;
                                    margin: 0;
                                    padding: 0;
                                    color: #333;
                                }
                                .email-container {
                                    background-color: white;
                                    max-width: 600px;
                                    margin: 0 auto;
                                    border-radius: 8px;
	                                border: 0.3px solid rgba(0, 0, 0, 0.2);
    	                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1),
                                        0 8px 16px rgba(0, 0, 0, 0.2);
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
                                    padding: 10px;
                                    color: #333;
                                }
                                .email-body h3 {
                                    color: rgb(255, 112, 129);
                                    font-size: 24px;
                                    font-weight: bold;
                                    margin-bottom: 15px;
	                            margin-top: 15px;
                                }
                                .email-body p {
                                    font-size: 16px;
                                    line-height: 1.6;
                                    margin: 10px 0;
                                }
                                .verify-button {
                                    padding: 10px 20px;
                                    text-decoration: none;
                                    font-size: 16px;
                                    color: #ffffff !important;
                                    background-color: #ff7081;
                                    text-decoration: none;
                                    border-radius: 5px;
                                    margin-top: 15px;
                                    text-align: center;
                                    margin-left: auto;
                                    margin-right: auto;
                                    width: 100px;
                                    display: block;
                                }
                                .verify-button:hover {
                                    opacity: 0.8;
                                }
                                .email-footer {
                                    padding: 20px;
                                    font-size: 14px;
                                    color: #888;
                                    border-top: 1px solid #eaeaea;
                                    text-align: center;
                                }
                                .email-footer p {
                                    margin: 15px 0;
                                }
                                .email-footer .signature {
                                    text-align: left;
                                    margin-bottom: 20px;
                                }
                                .email-footer .copyright {
                                    font-size: 12px;
                                    color: #999;
                                    text-align: center;
                                }
                            </style>
                        </head>
                        <body>
                            <div class=""email-container"">
                                <div class=""email-header"">
                                    Thông báo Xác Nhận Email
                                </div>
                                <div class=""email-body"">
                                    <h3>Kính chào khách hàng thân mến,</h3>
                                    <p>Email này được gửi để xác nhận rằng chúng tôi đã nhận được yêu cầu <strong>{Action}</strong> của bạn và địa chỉ email này hiện đang được xác minh.</p>
                                    <p>Nếu bạn đã yêu cầu thay đổi hoặc cập nhật thông tin, vui lòng nhấn vào nút dưới đây để hoàn tất quá trình xác minh:</p>
                                    <a href=""{confirmLink}"" class=""verify-button"">Xác nhận</a>
                                    <p><strong>Lưu ý:</strong> Liên kết xác minh có hiệu lực trong 10 phút.</p>
                                    <p>Vui lòng không gửi liên kết này cho người khác vì lý do bảo mật thông tin.</p>
                                    <p>Nếu bạn không yêu cầu xác nhận này, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi để được hỗ trợ.</p>
                                </div>
                                <div class=""email-footer"">
                                    <div class=""signature"">
                                        <p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi.</p>
                                        <p>Trân trọng,<br><br>{projectName}</p>
                                    </div>
                                    <div class=""copyright"">
                                        <p>© 2024 Bản quyền thuộc về {projectName}.</p>
                                    </div>
                                </div>
                            </div>
                        </body>
                        </html>
                        ";
        #endregion

        #region uploadDataErrorTemplate
        public string uploadDataErrorTemplate = @"<!DOCTYPE html>
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
                                                    border: 0.3px solid rgba(0, 0, 0, 0.2);
                                                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
                                                }
                                                .header {
                                                    background-color: rgb(255, 112, 129);
                                                    color: white;
                                                    text-align: center;
                                                    padding: 10px;
                                                    font-size: 20px;
                                                    font-weight: bold;
                                                }
                                        h1{
                                         margin: 15px;
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
                                        </html>
                                        ";
        #endregion

        #region influencerOffer
        public string influencerOffer = @"
                        <!DOCTYPE html>
                        <html lang=""vi"">
                          <head>
                            <meta charset=""UTF-8"" />
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                            <title>Thông Báo Nhà sáng tạo nội dung Tham Gia Chiến Dịch</title>
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
                                border: 0.3px solid rgba(0, 0, 0, 0.2);
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                              <div class=""email-header"">Thông Báo Nhà sáng tạo nội dung Tham Gia Chiến Dịch</div>
                              <div class=""email-body"">
                                <h2>Xin chào {BrandName},</h2>
                                <p>
                                  Chúng tôi xin thông báo rằng Nhà sáng tạo nội dung
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
                                  Vui lòng xem xét và phản hồi để xác nhận sự tham gia của Nhà sáng tạo nội dung.
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

        #region brandOffer
        public string brandOffer = @"
                            <!DOCTYPE html>
                            <html lang=""vi"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Đề nghị Công việc</title>
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
                                        border: 0.3px solid rgba(0, 0, 0, 0.2);
                                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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

        #region confirmOffer
        public string confirmOffer = @"<!DOCTYPE html>
                            <html lang=""vi"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Thông Báo trạng thái Đề nghị</title>
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
	                                border: 0.3px solid rgba(0, 0, 0, 0.2);
                                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                                    <div class=""email-header"">
                                        {Title} 
                                    </div>
                                    <div class=""email-body"">
                                        <h2>Xin chào {Name},</h2>
                                        <p>Chúng tôi xin thông báo rằng <strong>{Actor}</strong> đã {Status} Đề nghị của bạn:</p>
                                        <p>Thông tin chi tiết về Đề nghị của bạn dành cho {Actor}:</p>
                                        <ul>
                                            <li>Yêu cầu của chiến dịch : {ContentType}</li>
                                            <li>Giá: {Price}</li>
                                            <li>Thời gian tạo: {CreatedAt}</li>
                                            <li>Thời gian phản hồi: {ResponseTime}</li>
                                            <li>Mô tả: {Description}</li>
                                        </ul>
                                        <p>Để biết thêm chi tiết, vui lòng truy cập vào:</p>
                                        <div class=""button-container"">
                                            <a href=""{DetailsLink}"" class=""button"">Xem Chi Tiết</a>
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
                            </html>";
        #endregion
        
        # region brandPaymentOffer
        public string brandPaymentOffer = @"<!DOCTYPE html>
                                        <html lang=""vi"">
                                        <head>
                                            <meta charset=""UTF-8"">
                                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                            <title>Thông Báo Nhãn hàng Đã Thanh Toán Đề nghị Thành Công</title>
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
                                                <div class=""email-header"">
                                                    Thông Báo Đề nghị đã {Title}
                                                </div>
                                                <div class=""email-body"">
                                                    <h2>Xin chào {InfluencerName},</h2>
                                                    <p>Chúng tôi xin thông báo rằng Nhãn hàng <strong>{BrandName}</strong> đã {Status} thanh toán Đề nghị</p>
                                                    <p>Thông tin chi tiết về Đề nghị của bạn:</p>
                                                    <ul>
                                                        <li>Yêu cầu của chiến dịch : {ContentType}</li>
                                                        <li>Giá: {Price}</li>
                                                        <li>Mô tả: {Description}</li>
                                                        <li>Thời gian tạo: {CreatedAt}</li>
                                                        <li>Thời gian phản hồi: {ResponseAt}</li>
                                                    </ul>
                                                    <p>{EndQuote}</p>
                                                    <div class=""button-container"">
                                                        <a href=""{ReportLink}"" class=""button"">Xem Chi Tiết</a>
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
                                        </html>";
        #endregion

        #region Report
        public string reportTemplate = @"<!DOCTYPE html>
                    <html lang=""vi"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Thông Báo Nhà sáng tạo nội dung Vi Phạm Chính Sách Trang Web</title>
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
                                border: 0.3px solid rgba(0, 0, 0, 0.2);
                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                            .email-footer .copyright {
                                text-align: center;
                            }
                        </style>
                    </head>
                    <body>
                        <div class=""email-container"">
                            <div class=""email-header"">
                                Thông Báo Nhà sáng tạo nội dung Vi Phạm Chính Sách Trang Web
                            </div>
                            <div class=""email-body"">
                                <h2>Xin chào Admin,</h2>
                                <p>Chúng tôi xin thông báo rằng Nhà sáng tạo nội dung <strong>{InfluencerName}</strong> đã vi phạm chính sách của trang web:</p>
                                <p><strong>{Reason}</strong></p>
                                <p>Thông tin chi tiết về sai phạm của Nhà sáng tạo nội dung:</p>
                                <ul>
                                    <li>Người báo cáo: {Reporter}</li>
                                    <li>Thời gian báo cáo: {CreatedAt}</li>
                                    <li>Mô tả: {Description}</li>
                                </ul>
                                <p>Vui lòng xem xét và phản hồi để có hướng xử lý phù hợp cho các hành vi vi phạm chính sách của Nhà sáng tạo nội dung.</p>
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
                    </html>";
        #endregion

        #region ReportResult
        public string reportResultTemplate = @"<!DOCTYPE html>
                                    <html lang=""vi"">
                                    <head>
                                        <meta charset=""UTF-8"">
                                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                        <title>Thông Báo Kết Quả Báo Cáo</title>
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
                                                border: 0.3px solid rgba(0, 0, 0, 0.2);
                                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                                                line-height: 1.2;
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
                                            .email-footer .copyright {
                                                text-align: center;
                                            }
                                        </style>
                                    </head>
                                    <body>
                                        <div class=""email-container"">
                                            <div class=""email-header"">
                                                Thông Báo Kết Quả Báo Cáo
                                            </div>
                                            <div class=""email-body"">
                                                <h2>Xin chào Quý khách hàng,</h2>
                                                <p>Chúng tôi xin thông báo rằng báo cáo của bạn cho Nhà sáng tạo nội dung: <strong>{Influencer}</strong>,với lý do <strong>{Reason}</strong> Đã được xem xét và <strong>{Status}</strong></p>
	                                            <p>Chúng tôi đánh giá cao sự đóng góp của bạn và khuyến khích bạn tiếp tục gửi báo cáo trong tương lai để giúp duy trì cộng đồng lành mạnh và an toàn.</p>
                                                <p>Nếu bạn muốn thảo luận thêm về kết quả này hoặc có bất kỳ thắc mắc nào, vui lòng liên hệ với chúng tôi. Đội ngũ hỗ trợ của chúng tôi sẽ sẵn sàng giúp đỡ bạn.</p>
 	                                        <p><strong>Lưu ý:</strong> Nếu chúng tôi phát hiện hành vi gửi báo cáo sai lệch hoặc spam, tài khoản của bạn có thể bị xử lý theo quy định của trang web, bao gồm việc tạm khóa hoặc đình chỉ vĩnh viễn.</p>
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

        #region ReportNotification
        public string reportNotificationTempalte = @"<!DOCTYPE html>
                        <html lang=""vi"">
                        <head>
                            <meta charset=""UTF-8"">
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                            <title>Thông Báo Tài Khoản Đã Bị Cấm</title>
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
                                    border: 0.3px solid rgba(0, 0, 0, 0.2);
                                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                                    line-height: 1.2;
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
                                <div class=""email-header"">
                                    Thông Báo Tài Khoản Đã Bị Cấm
                                </div>
                                <div class=""email-body"">
                                    <h2>Xin chào Quý khách hàng,</h2>
                                    <p>Chúng tôi xin thông báo rằng tài khoản của bạn đã bị cấm do vi phạm quy định của trang web.</p>
                                    <p> Lý do cụ thể: <strong>{Description}</strong>.</p>
                                    <p> Thời gian bị cấm : <strong>{CurrenDate}</strong>.</p>
                                    <p> Thời gian mở khóa: <strong>{UnbanDate}</strong>.</p>
                                    <p>Chúng tôi đánh giá cao sự đóng góp của bạn, nhưng việc duy trì một cộng đồng lành mạnh và an toàn là ưu tiên hàng đầu của chúng tôi. Nếu bạn có bất kỳ câu hỏi nào liên quan đến quyết định này, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
                                    <p><strong>Lưu ý:</strong> Nếu bạn tin rằng việc cấm tài khoản là không công bằng, bạn có thể gửi yêu cầu xem xét lại. Tuy nhiên, hành vi gửi báo cáo sai lệch hoặc spam có thể dẫn đến các biện pháp xử lý nghiêm khắc hơn.</p>
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
        public string unbanNotification = @"<!DOCTYPE html>
                                <html lang=""vi"">
                                <head>
                                    <meta charset=""UTF-8"">
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                    <title>Thông Báo Tài Khoản Đã Được Mở Khóa</title>
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
                                            border: 0.3px solid rgba(0, 0, 0, 0.2);
                                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                                            line-height: 1.2;
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
                                        <div class=""email-header"">
                                            Thông Báo Tài Khoản Đã Được Mở Khóa
                                        </div>
                                        <div class=""email-body"">
                                            <h2>Xin chào Quý khách hàng,</h2>
                                            <p>Chúng tôi xin thông báo rằng tài khoản của bạn đã được mở khóa sau khi xem xét.</p>
                                            <p> Tài khoản của bạn đã bị cấm trước đó vào ngày: <strong>{BanDate}</strong>.</p>
                                            <p> Thời gian mở khóa: <strong>{UnbanDate}</strong>.</p>                                           
                                            <p> Mô tả: <strong>{Description}</strong>.</p>
                                            <p>Chúng tôi rất mong bạn tiếp tục tham gia vào cộng đồng của chúng tôi và duy trì các quy tắc để đảm bảo một môi trường an toàn và lành mạnh.</p>    
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

        #region CampaignStart
        public string campaignStart = @"<!DOCTYPE html>
                                        <html lang=""vi"">
                                        <head>
                                            <meta charset=""UTF-8"">
                                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                            <title>Thông Báo Chiến dịch Đã Bắt Đầu</title>
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
                                                    border: 0.3px solid rgba(0, 0, 0, 0.2);
                                                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                                                <div class=""email-header"">
                                                    Thông Báo Chiến dịch Đã Bắt Đầu
                                                </div>
                                                <div class=""email-body"">
                                                    <h2>Xin chào Inluencer,</h2>
                                                    <p>Chúng tôi xin thông báo rằng Chiến dịch <strong>{CampaignName}</strong> mà bạn tham gia đã được bắt đầu.</p>
                                                    <p>Vui lòng chuẩn bị và triển khai công việc theo như đã thỏa thuận trước đó.</p>
                                                    <p>Thông tin chi tiết về Chiến dịch của bạn:</p>
                                                    <ul>
                                                        <li>Chiến dịch: {CampaignName} - {Title}</li>
                                                        <li>Nhãn hàng: {BrandName}</li>
                                                        <li>Thời gian bắt đầu: {StartDate}</li>
                                                        <li>Thời gian dự kiến kết thúc: {EndDate}</li>
                                                    </ul>
                                                    <div class=""button-container"">
                                                        <a href=""{CampaignLink}"" class=""button"">Xem Chi Tiết</a>
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

        #region MeetingNotification
        public string meetingNotificationTemplate = @"<!DOCTYPE html>
                                    <html lang=""vi"">
                                    <head>
                                        <meta charset=""UTF-8"">
                                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                        <title>Thông Báo Cuộc Họp Sắp Diễn Ra</title>
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
                                                border: 0.3px solid rgba(0, 0, 0, 0.2);
                                                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1), 0 8px 16px rgba(0, 0, 0, 0.2);
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
                                                color: white !important;
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
                                            <div class=""email-header"">
                                                Thông Báo Cuộc Họp Sắp Diễn Ra
                                            </div>
                                            <div class=""email-body"">
                                                <h2>Xin chào Nhà sáng tạo nội dung,</h2>
                                                <p>Nhãn hàng <strong>{BrandName}</strong> đã tạo một cuộc họp cho chiến dịch <strong>{CampaignName}</strong>.</p>
                                                <p>Vui lòng tham gia đúng giờ để thảo luận và hoàn thành các nội dung quan trọng.</p>
                                                <p>Thông tin chi tiết về cuộc họp:</p>
                                                <ul>
                                                    <li>Chiến dịch: {CampaignName}</li>
                                                    <li>Nhãn hàng: {BrandName}</li>
                                                    <li>Thời gian bắt đầu: {StartTime}</li>
                                                    <li>Thời gian kết thúc: {EndTime}</li>
		                                    <li>Mô tả: {Description}</li>
                                                </ul>
	                                        <p>
		                                    <strong>Lưu ý quan trọng:</strong> Việc tham gia cuộc họp này là rất cần thiết để đảm bảo tiến độ và chất lượng công việc của chiến dịch 
		                                    <strong>{CampaignName}</strong>. Đây là cơ hội để trao đổi, cập nhật thông tin, và giải quyết các vấn đề cần thiết. 
		                                    Sự vắng mặt của bạn có thể ảnh hưởng đến tiến độ chung, vì vậy vui lòng đảm bảo có mặt đúng giờ để cuộc họp diễn ra suôn sẻ.
	                                        </p>
                                                <div class=""button-container"">
                                                    <a href=""{MeetingLink}"" class=""button"">Tham Gia Cuộc Họp</a>
                                                </div>
                                            </div>
                                            <div class=""email-footer"">
                                                <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                                                <p>Trân trọng,<br><br>{ProjectName}</p>
                                                <div class=""copyright"">
                                                    <p>© 2024 Bản quyền thuộc về {ProjectName}.</p>
                                                </div>
                                            </div>
                                        </div>
                                    </body>
                                    </html>
                                    ";
        #endregion
    }
}
