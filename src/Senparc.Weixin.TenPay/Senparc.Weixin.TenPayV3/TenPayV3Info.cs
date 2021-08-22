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
 
    文件名：TenPayV3Info.cs
    文件功能描述：微信支付V3基础信息储存类
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口

    修改标识：Senparc - 20180707
    修改描述：添加支持 SenparcWeixinSetting 参数的构造函数

    修改标识：Senparc - 20180802
    修改描述：v15.2.0 SenparcWeixinSetting 添加 TenPayV3_WxOpenTenpayNotify 属性，用于设置小程序支付回调地址

    修改标识：Senparc - 20190521
    修改描述：v1.4.0 .NET Core 添加多证书注册功能

----------------------------------------------------------------*/

using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Trace;
using Senparc.Weixin.Entities;
using Senparc.Weixin.TenPayV3.Apis;
using Senparc.Weixin.TenPayV3.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Senparc.Weixin.TenPayV3
{
    /// <summary>
    /// 微信支付基础信息储存类
    /// </summary>
    public class TenPayV3Info
    {
        private PublicKeyCollection publicKeys;

        /// <summary>
        /// 第三方用户唯一凭证appid
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 第三方用户唯一凭证密钥，即appsecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 商户支付密钥Key。登录微信商户后台，进入栏目【账户设置】【密码安全】【API 安全】【API 密钥】
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 微信支付证书位置（物理路径），在 .NET Core 下执行 TenPayV3InfoCollection.Register() 方法会为 HttpClient 自动添加证书
        /// </summary>
        public string CertPath { get; set; }
        /// <summary>
        /// 微信支付证书密码
        /// </summary>
        public string CertSecret { get; set; }
        /// <summary>
        /// 支付完成后的回调处理页面
        /// </summary>
        public string TenPayV3Notify { get; set; } // = "http://localhost/payNotifyUrl.aspx";
        /// <summary>
        /// 小程序支付完成后的回调处理页面
        /// </summary>
        public string TenPayV3_WxOpenNotify { get; set; }

        /// <summary>
        /// 服务商模式下，特约商户的开发配置中的AppId
        /// </summary>
        public string Sub_AppId { get; set; }
        /// <summary>
        /// 服务商模式下，特约商户的开发配置中的AppSecret
        /// </summary>
        public string Sub_AppSecret { get; set; }
        /// <summary>
        /// 服务商模式下，特约商户的商户Id
        /// </summary>
        public string Sub_MchId { get; set; }

        /// <summary>
        /// 普通服务商 微信支付 V3 参数 构造函数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="mchId"></param>
        /// <param name="key"></param>
        /// <param name="certPath"></param>
        /// <param name="certSecret"></param>
        /// <param name="tenPayV3Notify"></param>
        /// <param name="tenPayV3WxOpenNotify"></param>
        public TenPayV3Info(string appId, string appSecret, string mchId, string key, string certPath, string certSecret, string tenPayV3Notify, string tenPayV3WxOpenNotify)
            : this(appId, appSecret, mchId, key, certPath, certSecret, "", "", "", tenPayV3Notify, tenPayV3WxOpenNotify)
        {

        }
        /// <summary>
        /// 服务商户 微信支付 V3 参数 构造函数
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="mchId"></param>
        /// <param name="key"></param>
        /// <param name="certPath"></param>
        /// <param name="certSecret"></param>
        /// <param name="subAppId"></param>
        /// <param name="subAppSecret"></param>
        /// <param name="subMchId"></param>
        /// <param name="tenPayV3Notify"></param>
        /// <param name="tenPayV3WxOpenNotify"></param>
        public TenPayV3Info(string appId, string appSecret, string mchId, string key, string certPath, string certSecret, string subAppId, string subAppSecret, string subMchId, string tenPayV3Notify, string tenPayV3WxOpenNotify)
        {
            AppId = appId;
            AppSecret = appSecret;
            MchId = mchId;
            Key = key;
            CertPath = certPath;
            CertSecret = certSecret;
            TenPayV3Notify = tenPayV3Notify;
            TenPayV3_WxOpenNotify = tenPayV3WxOpenNotify;
            Sub_AppId = subAppId;
            Sub_AppSecret = subAppSecret;
            Sub_MchId = subMchId;
        }

        /// <summary>
        /// 微信支付 V3 参数 构造函数
        /// </summary>
        /// <param name="senparcWeixinSetting">已经填充过微信支付（旧版本）参数的 SenparcWeixinSetting 对象</param>
        public TenPayV3Info(ISenparcWeixinSettingForTenpayV3 senparcWeixinSetting)
            : this(senparcWeixinSetting.TenPayV3_AppId,
                  senparcWeixinSetting.TenPayV3_AppSecret,
                  senparcWeixinSetting.TenPayV3_MchId,
                  senparcWeixinSetting.TenPayV3_Key,
                  senparcWeixinSetting.TenPayV3_CertPath,
                  senparcWeixinSetting.TenPayV3_CertSecret,
                  senparcWeixinSetting.TenPayV3_SubAppId,
                  senparcWeixinSetting.TenPayV3_SubAppSecret,
                  senparcWeixinSetting.TenPayV3_SubMchId,
                  senparcWeixinSetting.TenPayV3_TenpayNotify,
                  senparcWeixinSetting.TenPayV3_WxOpenTenpayNotify
                  )
        {
        }

        /// <summary>
        /// 获取当前支付账号下所有公钥信息
        /// </summary>
        public async Task<PublicKeyCollection> GetPublicKeysAsync()
        {
            //TODO:可以升级为从缓存读取

            if (publicKeys == null)
            {
                //获取最新的 Key
                publicKeys = await BasePayApis.GetPublicKeysAsync();
            }
            return publicKeys;
        }

        /// <summary>
        /// 获取单个公钥
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public async Task<string> GetPublicKeyAsync(string serialNumber)
        {
            var keys = await GetPublicKeysAsync();
            if (keys.TryGetValue(serialNumber, out string publicKey))
            {
                return publicKey;
            }

            SenparcTrace.BaseExceptionLog(new TenpaySecurityException($"公钥序列号不存在！serialNumber:{serialNumber},TenPayV3Info:{this.ToJson(true)}"));
            throw new TenpaySecurityException("公钥序列号不存在！请查看日志！", true);
        }
    }
}
