using ZXing;
using UnityEngine;
using ZXing.QrCode;

namespace OriginUtility
{
    public partial class Utility
    {
        /// <summary>
        /// 二维码工具
        /// </summary>
        public static class QRCodeUtility
        {
            /// <summary>
            /// 创建二维码
            /// </summary>
            /// <param name="content">内容</param>
            /// <param name="width">宽度</param>
            /// <param name="height">高度</param>
            /// <returns></returns>
            public static Sprite GeneratorQRCode(string content , int width = 256 , int height = 256)
            {
                Texture2D texture2D = GetQRCode(content , width , height);

                return Sprite.Create(texture2D , new Rect(0 , 0 , width , height) , new Vector2(0.5f , 0.5f));
            }

            /// <summary>
            /// 生成二维码的参数
            /// </summary>
            /// <param name="formatStr">字符串</param>
            /// <param name="width">宽</param>
            /// <param name="height">高</param>
            /// <returns></returns>
            private static Texture2D GetQRCode(string str , int width , int height)
            {
                // 创建一个新的Texture2D对象，根据传入的宽和高进行初始化
                Texture2D texture = new Texture2D(width , height);
                // 调用GeneQRCode方法生成二维码的颜色数组
                Color32[] colors = GenenQRCode(str , width , height);
                // 将生成的颜色数组设置到纹理中
                texture.SetPixels32(colors);
                // 应用纹理的修改（更新）
                texture.Apply( );
                // 返回生成的二维码纹理
                return texture;
            }
            /// <summary>
            /// 生成二维码的参数
            /// </summary>
            /// <param name="formatStr">字符串</param>
            /// <param name="width">宽</param>
            /// <param name="height">高</param>
            /// <returns></returns>
            private static Color32[] GenenQRCode(string formatStr , int width , int height)
            {
                // 创建QrCodeEncodingOptions对象，用于设置二维码编码参数
                QrCodeEncodingOptions options = new QrCodeEncodingOptions
                {
                    CharacterSet = "UTF-8" , // 设置字符编码，确保字符串信息保持正确
                    Width = width , // 设置二维码宽度
                    Height = height , // 设置二维码高度
                    Margin = 1 // 设置二维码留白（值越大，留白越大，二维码越小）
                };
                return new BarcodeWriter { Format = BarcodeFormat.QR_CODE , Options = options }.Write(formatStr); // 实例化字符串绘制二维码工具
            }
        }
    }
}
