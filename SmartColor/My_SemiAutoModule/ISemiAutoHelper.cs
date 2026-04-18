using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_SemiAutoModule
{
    // 1. 每个操作专属的返回枚举
    // 定义各类半自动操作的结果码，便于区分不同操作的返回状态



    /// <summary>
    /// 半自动操作统一结果级别码
    /// </summary>
    public enum SemiAutoResultCode
    {
        /// <summary>成功</summary>
        Success = 0,
        /// <summary>异常</summary>
        Exception = 1,
        /// <summary>需交互</summary>
        NeedInteraction = 2,
        /// <summary>需要使用</summary>
        NeedUseing = 3,
        /// <summary>需要机械复位</summary>
        MechanicalReset = 4, 
    }
    // 2. 统一的业务级别包装结构体

    /// <summary>
    /// 半自动操作统一结果结构体，封装操作级别、消息、原始码和异常信息
    /// </summary>
    public class SemiAutoResult
    {
        /// <summary>
        /// 级别码
        /// </summary>
        public SemiAutoResultCode Level { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string Message { get; set; }



        /// <summary>
        /// 异常信息（等具体通讯协议出来补上去）
        /// </summary>
        public Exception Exception { get; set; }


    }



    /// <summary>
    /// 半自动操作统一接口，定义所有半自动动作方法
    /// </summary>
    public interface ISemiAutoHelper
    {


        /// <summary>
        /// 回原点
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> HomeAsync();

        /// <summary>
        /// 抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="z">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <param name="currentWeight">当前重量</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> AspirateAsync(int z, int purgeCount, short syringeType, short currentWeight);

        /// <summary>
        /// 注液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <param name="dispenseHeight">注液高度（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> DispenseAsync(int z, int syringeType, short dispenseHeight);

        /// <summary>
        /// 加水
        /// </summary>
        /// <param name="objectWeight">目标重量</param>
        /// <param name="waterTime">加水时间/s</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> AddWaterAsync(double objectWeight, double waterTime);

        /// <summary>
        /// 放针
        /// </summary>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> ReleaseNeedleAsync(int syringeType);

        /// <summary>
        /// 泄压
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> DecompressAsync();

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="axis">轴号 0：X 1:Y 2:Z 3:转盘</param>
        /// <param name="pulse">坐标</param>
        /// <param name="hSpeed">速度</param>
        /// <param name="upSpeed">加减速</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> RelativeMoveAsync(short axis, int pulse, int hSpeed, int upSpeed);

        /// <summary>
        /// 复位
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> ResetAsync();

        /// <summary>
        /// 动作检查
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> ActionCheckAsync();

        /// <summary>
        /// 开盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> OpenLidAsync(short lockSignal);

        /// <summary>
        /// 放盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> PutLidAsync();

        /// <summary>
        /// 取盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> TakeLidAsync();

        /// <summary>
        /// 关盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> CloseLidAsync(short lockSignal);

        /// <summary>
        /// 机械手抓取
        /// </summary>
        /// <param name="graspType">抓取类型（0=干布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐 6=湿布夹）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> RobotHandGraspingAsync(short graspType, short syringeType);

        /// <summary>
        /// 机械手松放
        /// </summary>
        /// <param name="graspType">抓取类型（0=干湿布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> RobotArmReleaseAsync(short graspType, short syringeType);

        /// <summary>
        /// 取干布
        /// </summary>
        /// <param name="takeClothCylinderPos">备布区取布气缸位置（0=慢速中取布、1=慢速中2取布）</param>
        /// <param name="over20gCompensatePulse">超20g布布框下探补偿脉冲（备布框类型）</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> TakeDryClothAsync(short takeClothCylinderPos, short over20gCompensatePulse);

        /// <summary>
        /// 取湿布
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> TakeWetClothAsync();

        /// <summary>
        /// 放干布
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> PutDryClothAsync();

        /// <summary>
        /// 放湿布
        /// </summary>
        /// <param name="outCylinderPos">放布时气缸位置</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> PutWetClothAsync(short outCylinderPos);

        /// <summary>
        /// UV抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> UVAspirateAsync(int z, short syringeType);

        /// <summary>
        /// 洗针筒
        /// </summary>
        /// <param name="syringeType">针筒类型</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> WashSyringeAsync(short syringeType);

        /// <summary>
        /// 加溶解剂
        /// </summary>
        /// <param name="solventTime">加溶解剂时间/s</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> AddSolventAsync(double solventTime);

        /// <summary>
        /// 撑盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> SupportCoverAsync();

        /// <summary>
        /// PH抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> PHAspirateAsync(int z, short syringeType);

        /// <summary>
        /// PH排液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> PHDrainAsync(int z);

        /// <summary>
        /// 取母液瓶
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> TakeMotherBottleAsync(int z, short takeHeight);

        /// <summary>
        /// 放母液瓶
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> PutMotherBottleAsync(int z, short takeHeight);

        /// <summary>
        /// 加粉
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> AddPowderAsync(int z);

        /// <summary>
        /// 加粉前气缸到阻挡位
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> BeforeAddPowderBlockAsync();

        /// <summary>
        /// 母液瓶区定点移动
        /// </summary>
        /// <param name="bottleNum">瓶号</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToBottleAsync(int bottleNum, int x, int y, short isTrayIn, int allowMove);

        /// <summary>
        /// 配液区定点移动
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToCupAsync(int cupNum, short needHome, int x, int y, short isTrayIn, int allowMove);

        /// <summary>
        /// 杯盖区定点移动
        /// </summary>
        /// <param name="lidNum">杯盖号</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToCupLidAsync(int lidNum, int x, int y, int allowMove, short isTrayIn = 2);

        /// <summary>
        /// 天平区定点移动
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToBalanceAsync(int x, int y, short isTrayIn = 1);

        /// <summary>
        /// 泄压区定点移动
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToDecompressionAsync(int cupNum, int x, int y, short isTrayIn = 1);

        /// <summary>
        /// 备布区定点移动
        /// </summary>
        /// <param name="clothNum">备布位号</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToPrepareClothAsync(int clothNum, short needHome, int x, int y, short isTrayIn = 2);

        /// <summary>
        /// 出布区定点移动
        /// </summary>
        /// <param name="clothNum">出布位号</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToOutClothAsync(int clothNum, int x, int y, short isTrayIn = 1);

        /// <summary>
        /// 干布夹子定点移动
        /// </summary>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToDryClampAsync(short needHome, int x, int y, int allowMove, short isTrayIn = 2);

        /// <summary>
        /// 湿布夹子定点移动
        /// </summary>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToWetClampAsync(short needHome, int x, int y, int allowMove, short isTrayIn = 2);

        /// <summary>
        /// 洗针区定点移动
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToWashAsync(int x, int y, short isTrayIn = 1);

        /// <summary>
        /// 待机位定点移动
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        Task<SemiAutoResult> MoveToStandbyAsync();
    }


}