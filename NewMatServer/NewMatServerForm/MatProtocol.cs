using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewMatServerForm
{
    public class MatProtocol
    {
        //封条操作
        public const int SEAL_HANDLE = 1;
        //物资用户登录
        public const int MAT_USER_LOGIN = 2;
        //装卸板货
        public const int CARGO_HANDLE = 3;
        //封条管理操作记录
        public const int SEAL_OPER_RECORD = 4;
        //取消装车
        public const int CARGO_CANCEL = 5;
        //板货回流
        public const int CARGO_RETURN = 6;
        //检查商品日期
        public const int PRODUCTION_DATE_CHECK = 7;
        //记录旧商品日期
        public const int OLD_DATE_RECORD = 8;
        //GoodsHandle登录
        public const int GH_USER_LOGIN = 9;
        //实物商品资料录入
        public const int GOODS_MSG_INPUT = 10;
        //撤销笼号
        public const int CARGO_DELETE = 11;
        //物资盘点
        public const int MATERIAL_CHECKING = 12;
        //查询板货配送商场
        public const int CARGO_QUERY = 13;
        //笼号绑定物资卡
        public const int CARGO_2_MATCARD = 14;
        //调场处理
        public const int TURNOVER_HANDLE = 15;
        //调场确认
        public const int TURNOVER_CONFIRM = 16;
        //调场封条操作
        public const int TURNOVER_SEAL_HANDLE = 17;
        //板货盘点更新
        public const int MATERIAL_UPDATE = 18;
        //取消盘点数
        public const int MATERIAL_CANCEL = 19;
        //修改物资卡对应的实物编号
        public const int ENTITY_NO_MODIFY = 20;
        //周转处理 司机确认
        public const int TURNOVER_DRIVER_CONFIRM = 21;
        //测试通信情况
        public const int TEST_CONNECT = 97;
        //更新配置文件
        public const int CONFIG_UPDATE = 98;
        //更新程序
        public const int APPLICATION_UPDATE = 99;
        //物资数据库的连接字符串
        public const string CONNECT_TTMAT = "Data Source=ttdata;user id=ttmat;password=ttmat789";
        //主营数据库的连接字符串
        public const string CONNECT_TTSHOP = "Data Source=orttshop;user id=ttshop;password=ttadmin456";
        //本地物资数据库的连接字符串
        public const string CONNECT_LOCALHOST = "Data Source=oras2;user id=ttmat;password=ttmat789";
        //物资程序的后台程序包名
        public const string PKG_TTMAT_NLSACN = "TTMAT_PKG.ttmat_nlscan";
        //日期监控程序的后台程序包名
        public const string PKG_CHECK_PRODUCTION_DATE = "TTSHOP_NLSCAN_PKG.check_production_date";
        public const string REMOTE_SERVER = "192.168.0.5";
    }
}
