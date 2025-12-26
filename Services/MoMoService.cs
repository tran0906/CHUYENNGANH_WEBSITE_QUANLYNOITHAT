using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Services
{
    public class MoMoConfig
    {
        public string PartnerCode { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public string IpnUrl { get; set; } = string.Empty;
        public string RequestType { get; set; } = "payWithMethod";
    }

    public class MoMoCreatePaymentRequest
    {
        public string partnerCode { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public string orderId { get; set; } = string.Empty;
        public string orderInfo { get; set; } = string.Empty;
        public string redirectUrl { get; set; } = string.Empty;
        public string ipnUrl { get; set; } = string.Empty;
        public string requestType { get; set; } = string.Empty;
        public string extraData { get; set; } = string.Empty;
        public string lang { get; set; } = "vi";
        public string signature { get; set; } = string.Empty;
    }

    public class MoMoCreatePaymentResponse
    {
        public string partnerCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public long responseTime { get; set; }
        public string message { get; set; } = string.Empty;
        public int resultCode { get; set; }
        public string payUrl { get; set; } = string.Empty;
        public string shortLink { get; set; } = string.Empty;
    }

    public class MoMoIpnRequest
    {
        public string partnerCode { get; set; } = string.Empty;
        public string orderId { get; set; } = string.Empty;
        public string requestId { get; set; } = string.Empty;
        public long amount { get; set; }
        public string orderInfo { get; set; } = string.Empty;
        public string orderType { get; set; } = string.Empty;
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; } = string.Empty;
        public string payType { get; set; } = string.Empty;
        public long responseTime { get; set; }
        public string extraData { get; set; } = string.Empty;
        public string signature { get; set; } = string.Empty;
    }

    public class MoMoService
    {
        private readonly MoMoConfig _config;
        private readonly HttpClient _httpClient;

        public MoMoService(IConfiguration configuration)
        {
            _config = new MoMoConfig
            {
                PartnerCode = configuration["MoMo:PartnerCode"] ?? "",
                AccessKey = configuration["MoMo:AccessKey"] ?? "",
                SecretKey = configuration["MoMo:SecretKey"] ?? "",
                ApiEndpoint = configuration["MoMo:ApiEndpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create",
                RedirectUrl = configuration["MoMo:RedirectUrl"] ?? "",
                IpnUrl = configuration["MoMo:IpnUrl"] ?? "",
                RequestType = configuration["MoMo:RequestType"] ?? "payWithMethod"
            };
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Tạo URL thanh toán MoMo
        /// </summary>
        public async Task<(bool Success, string PayUrl, string Message)> CreatePaymentAsync(
            string orderId, 
            decimal amount, 
            string orderInfo,
            string? extraData = "",
            string? redirectUrl = null,
            string? ipnUrl = null)
        {
            try
            {
                var requestId = Guid.NewGuid().ToString();
                var amountLong = (long)amount;
                
                // Sử dụng URL từ tham số hoặc config
                var actualRedirectUrl = redirectUrl ?? _config.RedirectUrl;
                var actualIpnUrl = ipnUrl ?? _config.IpnUrl;

                // Tạo raw signature theo format MoMo yêu cầu
                var rawSignature = $"accessKey={_config.AccessKey}" +
                    $"&amount={amountLong}" +
                    $"&extraData={extraData}" +
                    $"&ipnUrl={actualIpnUrl}" +
                    $"&orderId={orderId}" +
                    $"&orderInfo={orderInfo}" +
                    $"&partnerCode={_config.PartnerCode}" +
                    $"&redirectUrl={actualRedirectUrl}" +
                    $"&requestId={requestId}" +
                    $"&requestType={_config.RequestType}";

                var signature = ComputeHmacSha256(rawSignature, _config.SecretKey);

                var request = new MoMoCreatePaymentRequest
                {
                    partnerCode = _config.PartnerCode,
                    requestId = requestId,
                    amount = amountLong,
                    orderId = orderId,
                    orderInfo = orderInfo,
                    redirectUrl = actualRedirectUrl,
                    ipnUrl = actualIpnUrl,
                    requestType = _config.RequestType,
                    extraData = extraData ?? "",
                    lang = "vi",
                    signature = signature
                };

                var jsonRequest = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_config.ApiEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var momoResponse = JsonSerializer.Deserialize<MoMoCreatePaymentResponse>(responseContent);

                if (momoResponse != null && momoResponse.resultCode == 0)
                {
                    return (true, momoResponse.payUrl, "Tạo thanh toán thành công");
                }

                return (false, "", momoResponse?.message ?? "Lỗi không xác định từ MoMo");
            }
            catch (Exception ex)
            {
                return (false, "", $"Lỗi kết nối MoMo: {ex.Message}");
            }
        }

        /// <summary>
        /// Xác thực chữ ký từ MoMo callback (IPN/Return)
        /// </summary>
        public bool VerifySignature(MoMoIpnRequest request)
        {
            var rawSignature = $"accessKey={_config.AccessKey}" +
                $"&amount={request.amount}" +
                $"&extraData={request.extraData}" +
                $"&message={request.message}" +
                $"&orderId={request.orderId}" +
                $"&orderInfo={request.orderInfo}" +
                $"&orderType={request.orderType}" +
                $"&partnerCode={request.partnerCode}" +
                $"&payType={request.payType}" +
                $"&requestId={request.requestId}" +
                $"&responseTime={request.responseTime}" +
                $"&resultCode={request.resultCode}" +
                $"&transId={request.transId}";

            var computedSignature = ComputeHmacSha256(rawSignature, _config.SecretKey);
            return computedSignature == request.signature;
        }

        /// <summary>
        /// Tính HMAC SHA256
        /// </summary>
        private string ComputeHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
