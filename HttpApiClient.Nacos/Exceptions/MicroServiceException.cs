using System;

namespace HttpApiClient.Nacos.Exceptions
{
    public class MicroServiceException : Exception
    {
        public string Msg { get; set; }


        public MicroServiceException(int status,params string[] param)
        {
            switch (status)
            {
                case MicroServiceException.FALL_BACK:
                    this.Msg = "fallback";
                    break;
                case STA_ERR:
                    this.Msg = string.Format("服务返回非正常状态码：状态码：{0}，json:{1}", param);
                    break;
                case SER_ERR:
                    this.Msg = "序列化错误:"+param[0];
                    break;
                case SERVICE_ERR:
                    this.Msg = string.Format("服务返回错误：状态码：{0}，message:{1}",param);
                    break;
                case ERROR:
                    this.Msg = "未知错误";
                    break;
            }
        }
        public const int ERROR = 0;
        public const int FALL_BACK = 1;
        public const int SER_ERR = 2;
        public const int STA_ERR = 3;
        public const int SERVICE_ERR = 4;
    }
}
