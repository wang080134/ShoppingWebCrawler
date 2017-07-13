﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCPMessage.EntityPackage.Arguments
{
    /// <summary>
    /// 【一淘】搜索页面 参数
    /// </summary>
    public class ETaoFetchWebPageArgument : BaseFetchWebPageArgument
    {
        /// <summary>
        /// 获取【一淘】平台支持的排序字段列表
        /// </summary>
        /// <returns></returns>
        public override List<OrderField> GetCurrentPlatformSupportOrderFields()
        {
            List<OrderField> fields = new List<OrderField>() {

                 new OrderField { DisplayName="综合", FieldValue="default" },
                 new OrderField { DisplayName="销量", FieldValue="sales_desc" },
                 new OrderField { DisplayName="价格", FieldValue="price_asc,price_desc" },
                
            };

            return fields;
        }
    }
}