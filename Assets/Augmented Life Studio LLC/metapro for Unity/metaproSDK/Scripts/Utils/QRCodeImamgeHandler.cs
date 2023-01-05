using System;
using QRCoder;
using QRCoder.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace metaproSDK.Scripts.Utils
{
    public class QRCodeImamgeHandler 
    {

        public static Sprite GenerateQRCode(string url)
        {
            Debug.Log("QR code to: " + url);
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            UnityQRCode qrCode = new UnityQRCode(qrCodeData);

            Texture2D qrCodeAsTexture2D = qrCode.GetGraphic(pixelsPerModule:10);

            qrCodeAsTexture2D.filterMode = FilterMode.Point;

            var qrCodeSprite = Sprite.Create(qrCodeAsTexture2D, 
                new Rect(0, 0, qrCodeAsTexture2D.width, qrCodeAsTexture2D.height), new Vector2(0.5f, 0.5f), 100f);
            return qrCodeSprite;
        }

    }
}