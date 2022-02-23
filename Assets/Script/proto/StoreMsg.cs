using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//关于商店的协议
public enum ProductType
{
      weapen,
}
public class MsgGetStoreData:MsgBase
{
      public MsgGetStoreData()
      {
            protoName = "MsgGetStoreData";
      }
      //商品类型(客户端发)
      public string productType;
      //商品信息(服务端发)
      public ProductData[] products;
}
public class MsgBuyProduct:MsgBase
{
      public MsgBuyProduct()
      {
            protoName = "MsgBuyProduct";
      }
      //商品名称客户端给
      public string productName;
      public int result;//0代表购买失败1代表购买成功
}
