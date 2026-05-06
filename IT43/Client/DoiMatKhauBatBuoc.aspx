<%@ Page Title="Đổi mật khẩu bắt buộc" Language="C#"
   CodeBehind="DoiMatKhauBatBuoc.aspx.cs" Inherits="Account_DoiMatKhauBatBuoc" %>
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width,initial-scale=1" />
    <title>Đổi mật khẩu bắt buộc — Tra cứu tuyển sinh</title>
    <link rel="stylesheet" href="<%: ResolveUrl("~/lib/bootstrap/bootstrap.min.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/lib/bootstrap/bootstrap-icons.css") %>" />
    <style>
        :root {
            --primary: #1976D2;
            --primary-dark: #0d47a1;
        }
        * { box-sizing: border-box; }
        body {
            background: linear-gradient(135deg, #e3f0ff 0%, #f5f7fa 50%, #e8f5e9 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Segoe UI', sans-serif;
        }
        .auth-wrapper {
            width: 100%;
            max-width: 440px;
            padding: 16px;
        }
        .brand-link {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            text-decoration: none;
            margin-bottom: 20px;
        }
        .brand-link .brand-icon {
            width: 36px;
            height: 36px;
            background: var(--primary);
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #fff;
            font-size: 1.1rem;
        }
        .brand-link .brand-name {
            font-size: 1rem;
            font-weight: 700;
            color: var(--primary);
            letter-spacing: 0.3px;
        }
        .card {
            border: none;
            border-radius: 16px;
            box-shadow: 0 8px 32px rgba(25,118,210,.13), 0 2px 8px rgba(0,0,0,.06);
        }
        .card-body { padding: 2rem 2.2rem; }
        .page-icon-wrap {
            width: 68px;
            height: 68px;
            background: linear-gradient(135deg, #fff8e1, #fffde7);
            border: 3px solid #ffe082;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 1rem;
            font-size: 2rem;
            color: #f59e0b;
            box-shadow: 0 4px 12px rgba(245,158,11,.2);
        }
        .page-title {
            font-size: 1.25rem;
            font-weight: 700;
            color: #1a2340;
            margin-bottom: .3rem;
        }
        .page-sub {
            font-size: .82rem;
            color: #6b7280;
            line-height: 1.5;
        }
        .form-label {
            font-weight: 600;
            font-size: .875rem;
            color: #374151;
            margin-bottom: .35rem;
        }
        .pass-wrap {
            position: relative;
        }
        .pass-wrap .form-control {
            padding-right: 2.75rem;
            border-radius: 8px;
            border: 1.5px solid #d1d5db;
            transition: border-color .2s, box-shadow .2s;
            font-size: .9rem;
        }
        .pass-wrap .form-control:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(25,118,210,.12);
        }
        .pass-toggle {
            position: absolute;
            right: 0;
            top: 0;
            height: 100%;
            width: 2.6rem;
            background: none;
            border: none;
            color: #6b7280;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            font-size: 1rem;
            transition: color .2s;
        }
        .pass-toggle:hover { color: var(--primary); }
        /* Strength bar */
        .strength-track {
            height: 4px;
            background: #e5e7eb;
            border-radius: 4px;
            margin-top: 6px;
            overflow: hidden;
        }
        .strength-fill {
            height: 100%;
            width: 0;
            border-radius: 4px;
            transition: width .35s, background .35s;
        }
        .strength-label {
            font-size: .75rem;
            margin-top: 3px;
            color: #9ca3af;
            min-height: 16px;
        }
        .btn-confirm {
            background: linear-gradient(135deg, #f59e0b, #f97316);
            border: none;
            color: #fff;
            font-weight: 700;
            font-size: .95rem;
            border-radius: 8px;
            padding: .65rem 1rem;
            width: 100%;
            transition: opacity .2s, transform .1s;
            letter-spacing: .3px;
        }
        .btn-confirm:hover { opacity: .92; transform: translateY(-1px); }
        .btn-confirm:active { transform: translateY(0); }
        .divider {
            border-top: 1px solid #e5e7eb;
            margin: 1.2rem 0 .9rem;
        }
        .back-link {
            font-size: .82rem;
            color: #6b7280;
            text-align: center;
        }
        .back-link a {
            color: var(--primary);
            text-decoration: none;
            font-weight: 600;
        }
        .back-link a:hover { text-decoration: underline; }
    </style>
</head>
<body>
<form id="form1" runat="server">
<div class="auth-wrapper">

    <!-- Brand -->
    <a href="/index.aspx" class="brand-link">
        <span class="brand-icon"><i class="bi bi-mortarboard-fill"></i></span>
        <span class="brand-name">Tra Cứu Tuyển Sinh</span>
    </a>

    <div class="card">
        <div class="card-body">

            <!-- Header -->
            <div class="text-center mb-4">
                <div class="page-icon-wrap">
                    <i class="bi bi-shield-lock-fill"></i>
                </div>
                <div class="page-title">Đổi mật khẩu bắt buộc</div>
                <div class="page-sub">
                    Tài khoản của bạn được tạo bởi quản trị viên.<br />
                    Vui lòng đặt mật khẩu mới trước khi tiếp tục.
                </div>
            </div>

            <!-- Alert -->
            <asp:Literal ID="litThongBao" runat="server" />

            <!-- Mật khẩu mới -->
            <div class="mb-3">
                <label class="form-label">Mật khẩu mới <span class="text-danger">*</span></label>
                <div class="pass-wrap">
                    <asp:TextBox ID="txtMK" runat="server" CssClass="form-control"
                        TextMode="Password" placeholder="Ít nhất 8 ký tự..." />
                    <button type="button" class="pass-toggle" onclick="togglePwd(this)" tabindex="-1">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
                <!-- Strength indicator -->
                <div class="strength-track">
                    <div class="strength-fill" id="strengthFill"></div>
                </div>
                <div class="strength-label" id="strengthLabel"></div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMK"
                    ErrorMessage="Vui lòng nhập mật khẩu mới." CssClass="text-danger small"
                    Display="Dynamic" ValidationGroup="ForcePwd" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMK"
                    ValidationExpression=".{8,}"
                    ErrorMessage="Mật khẩu phải có ít nhất 8 ký tự." CssClass="text-danger small"
                    Display="Dynamic" ValidationGroup="ForcePwd" />
            </div>

            <!-- Xác nhận mật khẩu -->
            <div class="mb-4">
                <label class="form-label">Xác nhận mật khẩu <span class="text-danger">*</span></label>
                <div class="pass-wrap">
                    <asp:TextBox ID="txtMK2" runat="server" CssClass="form-control"
                        TextMode="Password" placeholder="Nhập lại mật khẩu..." />
                    <button type="button" class="pass-toggle" onclick="togglePwd(this)" tabindex="-1">
                        <i class="bi bi-eye"></i>
                    </button>
                </div>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMK2"
                    ErrorMessage="Vui lòng xác nhận mật khẩu." CssClass="text-danger small"
                    Display="Dynamic" ValidationGroup="ForcePwd" />
                <asp:CompareValidator runat="server"
                    ControlToValidate="txtMK2" ControlToCompare="txtMK"
                    ErrorMessage="Mật khẩu xác nhận không khớp." CssClass="text-danger small"
                    Display="Dynamic" ValidationGroup="ForcePwd" />
            </div>

            <!-- Submit -->
            <asp:Button ID="btnLuu" runat="server" Text="Xác nhận đổi mật khẩu"
                CssClass="btn-confirm" OnClick="btnLuu_Click"
                ValidationGroup="ForcePwd" />

            <div class="divider"></div>
            <div class="back-link">
                Không phải tài khoản của bạn? <a href="/logout.aspx">Đăng xuất</a>
            </div>

        </div>
    </div>
</div>
</form>

<script src="<%: ResolveUrl("~/lib/bootstrap/bootstrap.bundle.min.js") %>"></script>
<script>
function togglePwd(btn) {
    var wrap = btn.closest('.pass-wrap');
    var inp = wrap.querySelector('input');
    var icon = btn.querySelector('i');
    if (inp.type === 'password') {
        inp.type = 'text';
        icon.className = 'bi bi-eye-slash';
    } else {
        inp.type = 'password';
        icon.className = 'bi bi-eye';
    }
}

// Password strength
document.getElementById('<%= txtMK.ClientID %>').addEventListener('input', function() {
    var v = this.value, score = 0;
    if (v.length >= 8)          score++;
    if (/[A-Z]/.test(v))        score++;
    if (/[0-9]/.test(v))        score++;
    if (/[^a-zA-Z0-9]/.test(v)) score++;

    var fill  = document.getElementById('strengthFill');
    var label = document.getElementById('strengthLabel');
    var colors = ['#ef4444','#f59e0b','#3b82f6','#22c55e'];
    var labels = ['Yếu','Trung bình','Khá mạnh','Mạnh'];
    var widths = ['25%','50%','75%','100%'];

    if (v.length === 0) {
        fill.style.width = '0';
        fill.style.background = '';
        label.textContent = '';
    } else {
        var idx = Math.max(0, score - 1);
        fill.style.width   = widths[idx];
        fill.style.background = colors[idx];
        label.textContent  = 'Độ mạnh: ' + labels[idx];
        label.style.color  = colors[idx];
    }
});
</script>
</body>
</html>
