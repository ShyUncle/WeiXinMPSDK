﻿#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2021 Jeffrey Su & Suzhou Senparc Network Technology Co.,Ltd.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.

Detail: https://github.com/JeffreySu/WeiXinMPSDK/blob/master/license.md

----------------------------------------------------------------*/
#endregion Apache License Version 2.0

/*----------------------------------------------------------------
    Copyright (C) 2021 Senparc
  
    文件名：TenPaySignHelper.cs
    文件功能描述：微信支付V3签名Helper类 可用于创建签名 验证签名
    
    
    创建标识：Senparc - 20210819

    
----------------------------------------------------------------*/


using Senparc.Weixin.Entities;
using Senparc.Weixin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Weixin.TenPayV3.Helpers
{
    public class TenPaySignHelper
    {
        /// <summary>
        /// 创建签名
        /// <para>https://pay.weixin.qq.com/wiki/doc/apiv3/wechatpay/wechatpay4_0.shtml</para>
        /// </summary>
        /// <param name="message">签名串</param>
        /// <param name="privateKey">签名私钥 可为空</param>
        /// <returns></returns>
        public static string CreateSign(string message, string privateKey = null)
        {
            privateKey ??= Senparc.Weixin.Config.SenparcWeixinSetting.TenPayV3_PrivateKey;

            // NOTE： 私钥不包括私钥文件起始的-----BEGIN PRIVATE KEY-----
            //        亦不包括结尾的-----END PRIVATE KEY-----
            //string privateKey = "{你的私钥}";
            byte[] keyData = Convert.FromBase64String(privateKey);
            using (CngKey cngKey = CngKey.Import(keyData, CngKeyBlobFormat.Pkcs8PrivateBlob))
            using (RSACng rsa = new RSACng(cngKey))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
        }

        // TODO: 待测试
        /// <summary>
        /// 获取调起支付所需的签名
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="nonceStr">随机串</param>
        /// <param name="package">格式：prepay_id={0}</param>
        /// <param name="senparcWeixinSettingForTenpayV3">可为空 为空将从Senparc.Weixin.Config获取</param>
        /// <returns></returns>
        public static string CreatePaySign(string timeStamp, string nonceStr, string package, ISenparcWeixinSettingForTenpayV3 senparcWeixinSettingForTenpayV3 = null)
        {
            senparcWeixinSettingForTenpayV3 ??= Senparc.Weixin.Config.SenparcWeixinSetting.TenpayV3Setting;

            var appId = senparcWeixinSettingForTenpayV3.TenPayV3_AppId;
            var privateKey = senparcWeixinSettingForTenpayV3.TenPayV3_PrivateKey;

            string contentForSign = $"{appId}\n{timeStamp}\n{nonceStr}\n{package}\n";

            return CreateSign(contentForSign, privateKey);
        }

        /// <summary>
        /// 检验签名，以确保回调是由微信支付发送。
        /// 签名规则见微信官方文档 https://pay.weixin.qq.com/wiki/doc/apiv3/wechatpay/wechatpay4_1.shtml。
        /// return bool
        /// </summary>
        /// <param name="wechatpayTimestamp">HTTP头中的应答时间戳</param>
        /// <param name="wechatpayNonce">HTTP头中的应答随机串</param>
        /// <param name="wechatpaySignature">HTTP头中的应答签名</param>
        /// <param name="content">应答报文主体</param>
        /// <param name="pubKey">平台公钥 可为空</param>
        /// <returns></returns>
        public static bool VerifyTenpaySign(string wechatpayTimestamp, string wechatpayNonce, string wechatpaySignature, string content, string pubKey)
        {
            string contentForSign = $"{wechatpayTimestamp}\n{wechatpayNonce}\n{content}\n";

            // NOTE： 私钥不包括私钥文件起始的-----BEGIN PRIVATE KEY-----
            //        亦不包括结尾的-----END PRIVATE KEY-----
            //string privateKey = "{你的私钥}";
            byte[] keyData = Convert.FromBase64String(pubKey);
            using (CngKey cngKey = CngKey.Import(keyData, CngKeyBlobFormat.Pkcs8PrivateBlob))
            using (RSACng rsa = new RSACng(cngKey))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(contentForSign);
                byte[] signature = System.Text.Encoding.UTF8.GetBytes(wechatpaySignature);

                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        /// <summary>
        /// 检验签名，以确保回调是由微信支付发送。
        /// 签名规则见微信官方文档 https://pay.weixin.qq.com/wiki/doc/apiv3/wechatpay/wechatpay4_1.shtml。
        /// return bool
        /// </summary>
        /// <param name="wechatpayTimestamp">HTTP头中的应答时间戳</param>
        /// <param name="wechatpayNonce">HTTP头中的应答随机串</param>
        /// <param name="wechatpaySignature">HTTP头中的应答签名</param>
        /// <param name="content">应答报文主体</param>
        /// <param name="pubKey">平台公钥 可为空</param>
        /// <returns></returns>
        public static async Task<bool> VerifyTenpaySign(string wechatpayTimestamp, string wechatpayNonce, string wechatpaySignature, string content, ISenparcWeixinSettingForTenpayV3 senparcWeixinSettingForTenpayV3)
        {
            string contentForSign = $"{wechatpayTimestamp}\n{wechatpayNonce}\n{content}\n";

            var tenpayV3InfoKey = TenPayHelper.GetRegisterKey(senparcWeixinSettingForTenpayV3.TenPayV3_MchId, senparcWeixinSettingForTenpayV3.TenPayV3_SubMchId);
            var serialNumber = "";
            var pubKey = await TenPayV3InfoCollection.Data[tenpayV3InfoKey].GetPublicKeyAsync(serialNumber);
            return VerifyTenpaySign(wechatpayTimestamp, wechatpayNonce, wechatpaySignature, content, pubKey);
        }
    }
}
