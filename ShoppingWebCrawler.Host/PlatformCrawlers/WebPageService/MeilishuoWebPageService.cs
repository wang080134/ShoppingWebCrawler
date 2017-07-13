﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Http;
using System.Net;

namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 美丽说搜索页面抓取
    /// </summary>
    public class MeilishuoWebPageService : BaseWebPageService
    {

        private const string mlsSiteUrl = "http://www.meilishuo.com/";

        private const string templateOfSearchUrl = "http://www.meilishuo.com/search/goods/?page=1&searchKey={0}&acm=3.mce.1_4_.17721.33742-33692.3Va85qoXxtYNA.mid_17721-lc_201";
        /// <summary>
        /// 美丽说请求客户端--保持静态单个实例，防止多次实例化 创建请求链接导致的性能损失
        /// 不要将这个字段  抽象出来 保持跟具体的类同步
        /// </summary>
        private static readonly CookedHttpClient mlsHttpClient;


        /// <summary>
        /// 请求地址-根据方法传递的参数 动态格式化
        /// </summary>
        protected override string TargetUrl
        {
            get;set;
        }



        

        public MeilishuoWebPageService()
        {
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MeilishuoWebPageService()
        {
            //初始化头信息
            var requestHeaders = GetCommonRequestHeaders();
            requestHeaders.Add("Referer", mlsSiteUrl);
            mlsHttpClient = new CookedHttpClient();
            HttpServerProxy.FormatRequestHeader(mlsHttpClient.Client.DefaultRequestHeaders, requestHeaders);

        }
        /// <summary>
        /// 查询网页
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public override string QuerySearchContent(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord))
            {
                return null;
            }
            //格式化一个查询地址
           
            this.TargetUrl = string.Format(templateOfSearchUrl, keyWord);

            //获取当前站点的Cookie
            CookieCollection cks = GlobalContext.SupportPlatformsCookiesContainer[mlsSiteUrl];
            mlsHttpClient.ChangeGlobleCookies(cks, mlsSiteUrl);

            string respText = this.QuerySearchContentResonseAsync(mlsHttpClient.Client).Result;

            return respText;
        }


    }
}