/**
 * Validation Helper - Website Nội Thất Việt
 * Các hàm validate dữ liệu nhập vào form
 */

const Validator = {
    // ========== REGEX PATTERNS ==========
    patterns: {
        phone: /^(0[3|5|7|8|9])+([0-9]{8})$/,           // SĐT Việt Nam 10 số
        email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,            // Email cơ bản
        maCode: /^[A-Za-z0-9_-]+$/,                      // Mã (chữ, số, _, -)
        noSpecialChars: /^[a-zA-ZÀ-ỹ0-9\s]+$/,         // Không ký tự đặc biệt
        onlyNumbers: /^[0-9]+$/,                         // Chỉ số
        password: /^(?=.*[a-zA-Z])(?=.*\d).{6,}$/,      // Mật khẩu: ít nhất 6 ký tự, có chữ và số
        username: /^[a-zA-Z0-9_]{3,50}$/                 // Tên đăng nhập: 3-50 ký tự, chữ số _
    },

    // ========== VALIDATION FUNCTIONS ==========
    
    // Kiểm tra rỗng
    isEmpty: function(value) {
        return value === null || value === undefined || value.toString().trim() === '';
    },

    // Kiểm tra độ dài chuỗi
    checkLength: function(value, min, max) {
        if (this.isEmpty(value)) return { valid: false, message: 'Không được để trống' };
        const len = value.trim().length;
        if (min && len < min) return { valid: false, message: `Tối thiểu ${min} ký tự` };
        if (max && len > max) return { valid: false, message: `Tối đa ${max} ký tự` };
        return { valid: true };
    },

    // Kiểm tra số điện thoại Việt Nam
    isValidPhone: function(phone) {
        if (this.isEmpty(phone)) return { valid: false, message: 'Số điện thoại không được để trống' };
        phone = phone.replace(/\s/g, ''); // Xóa khoảng trắng
        if (!this.patterns.phone.test(phone)) {
            return { valid: false, message: 'SĐT phải 10 số, bắt đầu bằng 03, 05, 07, 08, 09' };
        }
        return { valid: true };
    },

    // Kiểm tra email
    isValidEmail: function(email) {
        if (this.isEmpty(email)) return { valid: true }; // Email có thể không bắt buộc
        if (!this.patterns.email.test(email)) {
            return { valid: false, message: 'Email không đúng định dạng (vd: abc@gmail.com)' };
        }
        return { valid: true };
    },

    // Kiểm tra mã (ID)
    isValidCode: function(code, fieldName = 'Mã') {
        if (this.isEmpty(code)) return { valid: false, message: `${fieldName} không được để trống` };
        if (code.length > 20) return { valid: false, message: `${fieldName} tối đa 20 ký tự` };
        if (!this.patterns.maCode.test(code)) {
            return { valid: false, message: `${fieldName} chỉ chứa chữ, số, dấu gạch ngang và gạch dưới` };
        }
        return { valid: true };
    },

    // Kiểm tra số dương
    isPositiveNumber: function(value, fieldName = 'Giá trị') {
        if (this.isEmpty(value)) return { valid: true }; // Có thể không bắt buộc
        const num = parseFloat(value);
        if (isNaN(num)) return { valid: false, message: `${fieldName} phải là số` };
        if (num < 0) return { valid: false, message: `${fieldName} không được âm` };
        return { valid: true };
    },

    // Kiểm tra số trong khoảng
    isInRange: function(value, min, max, fieldName = 'Giá trị') {
        if (this.isEmpty(value)) return { valid: true };
        const num = parseFloat(value);
        if (isNaN(num)) return { valid: false, message: `${fieldName} phải là số` };
        if (num < min || num > max) {
            return { valid: false, message: `${fieldName} phải từ ${min.toLocaleString()} đến ${max.toLocaleString()}` };
        }
        return { valid: true };
    },

    // Kiểm tra mật khẩu
    isValidPassword: function(password) {
        if (this.isEmpty(password)) return { valid: false, message: 'Mật khẩu không được để trống' };
        if (password.length < 6) return { valid: false, message: 'Mật khẩu tối thiểu 6 ký tự' };
        if (password.length > 100) return { valid: false, message: 'Mật khẩu tối đa 100 ký tự' };
        return { valid: true };
    },

    // Kiểm tra xác nhận mật khẩu
    isPasswordMatch: function(password, confirmPassword) {
        if (password !== confirmPassword) {
            return { valid: false, message: 'Mật khẩu xác nhận không khớp' };
        }
        return { valid: true };
    },

    // Kiểm tra họ tên
    isValidName: function(name) {
        if (this.isEmpty(name)) return { valid: false, message: 'Họ tên không được để trống' };
        if (name.length < 2) return { valid: false, message: 'Họ tên tối thiểu 2 ký tự' };
        if (name.length > 100) return { valid: false, message: 'Họ tên tối đa 100 ký tự' };
        return { valid: true };
    },

    // Kiểm tra địa chỉ
    isValidAddress: function(address) {
        if (this.isEmpty(address)) return { valid: false, message: 'Địa chỉ không được để trống' };
        if (address.length < 10) return { valid: false, message: 'Địa chỉ tối thiểu 10 ký tự' };
        if (address.length > 500) return { valid: false, message: 'Địa chỉ tối đa 500 ký tự' };
        return { valid: true };
    },

    // Kiểm tra ngày
    isValidDate: function(dateStr, fieldName = 'Ngày') {
        if (this.isEmpty(dateStr)) return { valid: false, message: `${fieldName} không được để trống` };
        const date = new Date(dateStr);
        if (isNaN(date.getTime())) {
            return { valid: false, message: `${fieldName} không hợp lệ` };
        }
        return { valid: true };
    },

    // Kiểm tra ngày trong tương lai
    isFutureDate: function(dateStr, fieldName = 'Ngày') {
        const result = this.isValidDate(dateStr, fieldName);
        if (!result.valid) return result;
        
        const date = new Date(dateStr);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        if (date < today) {
            return { valid: false, message: `${fieldName} phải từ hôm nay trở đi` };
        }
        return { valid: true };
    },

    // Kiểm tra khoảng ngày
    isValidDateRange: function(startDate, endDate) {
        if (this.isEmpty(startDate) || this.isEmpty(endDate)) {
            return { valid: false, message: 'Vui lòng chọn đầy đủ ngày bắt đầu và kết thúc' };
        }
        const start = new Date(startDate);
        const end = new Date(endDate);
        if (end < start) {
            return { valid: false, message: 'Ngày kết thúc phải sau ngày bắt đầu' };
        }
        return { valid: true };
    },

    // ========== FORM VALIDATION ==========
    
    // Hiển thị lỗi cho input
    showError: function(input, message) {
        const formGroup = input.closest('.form-group, .mb-3, .col-md-6, .col-md-4');
        if (!formGroup) return;
        
        // Xóa lỗi cũ
        this.clearError(input);
        
        // Thêm class lỗi
        input.classList.add('is-invalid');
        input.classList.remove('is-valid');
        
        // Tạo thông báo lỗi
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback d-block';
        errorDiv.textContent = message;
        
        // Chèn sau input
        input.parentNode.insertBefore(errorDiv, input.nextSibling);
    },

    // Xóa lỗi
    clearError: function(input) {
        input.classList.remove('is-invalid');
        const formGroup = input.closest('.form-group, .mb-3, .col-md-6, .col-md-4');
        if (formGroup) {
            const errorDiv = formGroup.querySelector('.invalid-feedback');
            if (errorDiv) errorDiv.remove();
        }
    },

    // Hiển thị thành công
    showSuccess: function(input) {
        this.clearError(input);
        input.classList.add('is-valid');
    },

    // Validate một input
    validateInput: function(input, rules) {
        const value = input.value;
        
        for (const rule of rules) {
            let result;
            
            switch (rule.type) {
                case 'required':
                    result = this.isEmpty(value) ? { valid: false, message: rule.message || 'Trường này là bắt buộc' } : { valid: true };
                    break;
                case 'length':
                    result = this.checkLength(value, rule.min, rule.max);
                    break;
                case 'phone':
                    result = this.isValidPhone(value);
                    break;
                case 'email':
                    result = this.isValidEmail(value);
                    break;
                case 'code':
                    result = this.isValidCode(value, rule.fieldName);
                    break;
                case 'positive':
                    result = this.isPositiveNumber(value, rule.fieldName);
                    break;
                case 'range':
                    result = this.isInRange(value, rule.min, rule.max, rule.fieldName);
                    break;
                case 'password':
                    result = this.isValidPassword(value);
                    break;
                case 'name':
                    result = this.isValidName(value);
                    break;
                case 'address':
                    result = this.isValidAddress(value);
                    break;
                case 'date':
                    result = this.isValidDate(value, rule.fieldName);
                    break;
                case 'futureDate':
                    result = this.isFutureDate(value, rule.fieldName);
                    break;
                case 'pattern':
                    result = rule.pattern.test(value) ? { valid: true } : { valid: false, message: rule.message };
                    break;
                case 'custom':
                    result = rule.validator(value);
                    break;
                default:
                    result = { valid: true };
            }
            
            if (!result.valid) {
                this.showError(input, result.message);
                return false;
            }
        }
        
        this.showSuccess(input);
        return true;
    },

    // Validate toàn bộ form
    validateForm: function(form, validationRules) {
        let isValid = true;
        
        for (const [fieldName, rules] of Object.entries(validationRules)) {
            const input = form.querySelector(`[name="${fieldName}"]`) || form.querySelector(`#${fieldName}`);
            if (input) {
                if (!this.validateInput(input, rules)) {
                    isValid = false;
                }
            }
        }
        
        return isValid;
    },

    // Gắn validation realtime cho form
    attachRealTimeValidation: function(form, validationRules) {
        for (const [fieldName, rules] of Object.entries(validationRules)) {
            const input = form.querySelector(`[name="${fieldName}"]`) || form.querySelector(`#${fieldName}`);
            if (input) {
                // Validate khi blur (rời khỏi input)
                input.addEventListener('blur', () => {
                    this.validateInput(input, rules);
                });
                
                // Xóa lỗi khi bắt đầu nhập
                input.addEventListener('input', () => {
                    this.clearError(input);
                });
            }
        }
        
        // Validate khi submit
        form.addEventListener('submit', (e) => {
            if (!this.validateForm(form, validationRules)) {
                e.preventDefault();
                // Scroll đến lỗi đầu tiên
                const firstError = form.querySelector('.is-invalid');
                if (firstError) {
                    firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    firstError.focus();
                }
            }
        });
    }
};

// ========== PRESET VALIDATION RULES ==========

// Rules cho form Đăng ký khách hàng
const RegisterValidationRules = {
    'hoTen': [
        { type: 'required', message: 'Họ tên không được để trống' },
        { type: 'name' }
    ],
    'soDienThoai': [
        { type: 'required', message: 'Số điện thoại không được để trống' },
        { type: 'phone' }
    ],
    'diaChi': [
        { type: 'required', message: 'Địa chỉ không được để trống' },
        { type: 'address' }
    ],
    'matKhau': [
        { type: 'required', message: 'Mật khẩu không được để trống' },
        { type: 'password' }
    ]
};

// Rules cho form Sản phẩm
const SanPhamValidationRules = {
    'Masp': [
        { type: 'required', message: 'Mã sản phẩm không được để trống' },
        { type: 'code', fieldName: 'Mã sản phẩm' },
        { type: 'length', min: 1, max: 20 }
    ],
    'Tensp': [
        { type: 'required', message: 'Tên sản phẩm không được để trống' },
        { type: 'length', min: 2, max: 200 }
    ],
    'Giaban': [
        { type: 'positive', fieldName: 'Giá bán' },
        { type: 'range', min: 0, max: 999999999, fieldName: 'Giá bán' }
    ],
    'Soluongton': [
        { type: 'positive', fieldName: 'Số lượng tồn' },
        { type: 'range', min: 0, max: 99999, fieldName: 'Số lượng tồn' }
    ]
};

// Rules cho form Nhà cung cấp
const NhaCungCapValidationRules = {
    'Mancc': [
        { type: 'required', message: 'Mã NCC không được để trống' },
        { type: 'code', fieldName: 'Mã NCC' }
    ],
    'Tenncc': [
        { type: 'required', message: 'Tên NCC không được để trống' },
        { type: 'length', min: 2, max: 200 }
    ],
    'Sdtncc': [
        { type: 'phone' }
    ],
    'Emailncc': [
        { type: 'email' }
    ],
    'Gianhap': [
        { type: 'positive', fieldName: 'Giá nhập' },
        { type: 'range', min: 0, max: 999999999, fieldName: 'Giá nhập' }
    ]
};

// Rules cho form User
const UserValidationRules = {
    'UserId': [
        { type: 'required', message: 'Mã người dùng không được để trống' },
        { type: 'code', fieldName: 'Mã người dùng' }
    ],
    'TenUser': [
        { type: 'required', message: 'Tên đăng nhập không được để trống' },
        { type: 'length', min: 3, max: 50 },
        { type: 'pattern', pattern: /^[a-zA-Z0-9_]+$/, message: 'Tên đăng nhập chỉ chứa chữ, số và dấu gạch dưới' }
    ],
    'HoTen': [
        { type: 'required', message: 'Họ tên không được để trống' },
        { type: 'name' }
    ],
    'MatKhau': [
        { type: 'required', message: 'Mật khẩu không được để trống' },
        { type: 'length', min: 6, max: 100 }
    ],
    'VaiTro': [
        { type: 'required', message: 'Vui lòng chọn vai trò' }
    ]
};

// Rules cho form Quảng bá
const QuanBaValidationRules = {
    'Madotgiamgia': [
        { type: 'required', message: 'Mã đợt giảm giá không được để trống' },
        { type: 'code', fieldName: 'Mã đợt giảm giá' }
    ],
    'Tendotgiamgia': [
        { type: 'required', message: 'Tên đợt giảm giá không được để trống' },
        { type: 'length', min: 2, max: 200 }
    ],
    'Phantramgiam': [
        { type: 'required', message: 'Phần trăm giảm không được để trống' },
        { type: 'range', min: 1, max: 100, fieldName: 'Phần trăm giảm' }
    ],
    'Ngaybatdau': [
        { type: 'required', message: 'Ngày bắt đầu không được để trống' },
        { type: 'date', fieldName: 'Ngày bắt đầu' }
    ],
    'Ngayketthuc': [
        { type: 'required', message: 'Ngày kết thúc không được để trống' },
        { type: 'date', fieldName: 'Ngày kết thúc' }
    ]
};

// Export cho sử dụng
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { Validator, RegisterValidationRules, SanPhamValidationRules, NhaCungCapValidationRules, UserValidationRules, QuanBaValidationRules };
}
