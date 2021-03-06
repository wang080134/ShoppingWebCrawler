﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net.Http;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common.Http;
using ShoppingWebCrawler.Host.Headless;
using ShoppingWebCrawler.Host.Common;


/*

 var etaoWeb = new TmallWebPageService();

            string con = etaoWeb.QuerySearchContent("mini裙子") ;

            System.Diagnostics.Debug.WriteLine(con);


*/
namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 天猫搜索页面抓取
    /// </summary>
    public class TmallWebPageService : BaseWebPageService
    {



        

        public TmallWebPageService()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return TmallMixReuestLoader.Current;
            }
        }




        ///------------内部类-----------------

        /// <summary>
        /// 天猫的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class TmallMixReuestLoader : BaseBrowserRequestLoader<TmallMixReuestLoader>
        {
            public static readonly string tmallSiteUrl = GlobalContext.SupportPlatforms.Find(x => x.Platform == SupportPlatformEnum.Tmall).SiteUrl;

            /// <summary>
            /// 天猫请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "https://list.tmall.com/search_product.htm?q={0}&type=p&vmarket=&spm=875.7931836%2FB.a2227oh.d100&xl=ip_1&from=mallfp..pc_1_suggest";

            /// <summary>
            /// 请求客户端
            /// </summary>
            private static  CookedHttpClient TmallHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static TmallMixReuestLoader()
            {
                //静态创建请求客户端
                //TmallHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "list.tmall.com");
                //requestHeaders.Add("Referer", TmallSiteUrl);
                TmallHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(TmallHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public TmallMixReuestLoader()
            {
                ///天猫刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate = "https://list.tmall.com/search_product.htm?spm=a220m.1000858.1000724.3.2a70033eTRXtEm&q={0}&sort=s&style=g&from=mallfp..pc_1_searchbutton#J_Filter" ;

                this.IntiCefWebBrowser();
            }

            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                string keyWord = queryParas.KeyWord;
                if (string.IsNullOrEmpty(keyWord))
                {
                    return string.Empty;
                }


                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(tmallSiteUrl);



                //天猫应该是有解析的搜索url的，如果没有，那么使用基于拼接的默认关键词的检索地址
                string searchUrl = "";
                if (null != queryParas.ResolvedUrl)
                {
                    searchUrl = queryParas.ResolvedUrl.Url;
                }
                else
                {
                  
                    string sortValue = "s";//综合排序
                    if (null!=queryParas.OrderFiled)
                    {
                        sortValue = queryParas.OrderFiled.FieldValue;
                    }
                  
                    searchUrl=string.Format(templateOfSearchUrl, keyWord, sortValue);
                }
                    

                var client = TmallHttpClient;

                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, tmallSiteUrl);
      
                // 4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };

                //注意：对于响应的内容 不要使用内置的文本 工具打开，这个工具有bug.看到的文本不全面
                //使用json格式打开即可看到里面所有的字符串

                string content = clientProxy.GetRequestTransfer(searchUrl, null);



                return content;

            }





        }

    }
}
