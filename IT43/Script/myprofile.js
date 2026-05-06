// MyProfile JavaScript - Tab switching, password toggle, file upload preview

// Tab switching functionality
function switchTab(tabName) {
    // Remove active class from all tabs
    document.querySelectorAll('.profile-tab').forEach(tab => {
        tab.classList.remove('active');
    });

    // Remove active class from all form sections
    document.querySelectorAll('.form-section').forEach(section => {
        section.classList.remove('active');
    });

    // Add active class to clicked tab
    event.target.closest('.profile-tab').classList.add('active');

    // Show corresponding form section
    const targetSection = document.getElementById('tab-' + tabName);
    if (targetSection) {
        targetSection.classList.add('active');
    }
}

// Password visibility toggle
function togglePassword(inputId, btn) {
    const input = document.getElementById(inputId);
    if (!input) return;

    if (input.type === 'password') {
        input.type = 'text';
        btn.innerHTML = '<i class="bi bi-eye-slash"></i>';
    } else {
        input.type = 'password';
        btn.innerHTML = '<i class="bi bi-eye"></i>';
    }
}

// File upload preview for avatar
document.addEventListener('DOMContentLoaded', function() {
    const fileInput = document.querySelector('.avatar-upload');
    const avatarImg = document.querySelector('.avatar-preview');

    if (fileInput && avatarImg) {
        fileInput.addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                // Validate file type
                const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
                if (!allowedTypes.includes(file.type)) {
                    alert('Chỉ chấp nhận file ảnh: JPG, PNG, GIF, WEBP');
                    fileInput.value = '';
                    return;
                }

                // Validate file size (5MB max)
                if (file.size > 5 * 1024 * 1024) {
                    alert('File ảnh không được vượt quá 5MB');
                    fileInput.value = '';
                    return;
                }

                // Preview image
                const reader = new FileReader();
                reader.onload = function(e) {
                    avatarImg.src = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        });
    }
});
