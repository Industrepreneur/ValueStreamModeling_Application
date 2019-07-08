using System.Collections.Generic;
using System.Web.Services;

public partial class API : System.Web.UI.Page
{
    public static readonly string myQuery = " SELECT zstblwhatif.familyid, tblgeneral.Title, tblProdFore.ProdDept, tblProdFore.ProdDesc, tblProdFore.ProdID, tblOper.OpNam, IIf(tblRsOper.WID=0,'Base Case','What-If Scenario') AS WID, tblOper.OpNum, tblLabor.LaborDesc, tblEquip.EquipDesc, tblRsOper.EqSetTime AS eqsettime, tblRsOper.EqRunTime AS eqruntime, tblRsOper.LabSetTime AS labSetTime, tblRsOper.LabRunTime AS labRunTime, tblRsOper.FlowTime, tblRsOper.WIP, tblRsOper.VisitsPer100, tblRsOper.VisitsPerGood, tblRsOper.NumSetups, tblRsOper.AverLotSize, tblOper.EqID, tblRsOper.LTEquip, tblRsOper.LTLabor, tblRsOper.LTSetup, tblRsOper.LTRun, tblRsOper.LTWaitLot, IIf([zstblwhatif].[display],[zstblwhatif].[name],'skip') AS description, tblRsOper.l_set_hours, tblRsOper.l_run_hours, tblRsOper.e_set_hours, tblRsOper.e_run_hours, tblRsOper.WID "
                           + " FROM tblgeneral, ((((tblProdFore INNER JOIN tblOper ON tblProdFore.ProdID = tblOper.ProdFore) INNER JOIN tblEquip ON tblOper.EqID = tblEquip.EquipID) INNER JOIN tblLabor ON tblEquip.Labor = tblLabor.LaborID) INNER JOIN tblRsOper ON (tblOper.OpID = tblRsOper.OpID) AND (tblProdFore.ProdID = tblRsOper.ProdID)) INNER JOIN zstblwhatif ON tblRsOper.WID = zstblwhatif.WID"
                           + " WHERE (((tblOper.OpNam)<>'dock' AND (tblOper.OpNam)<>'STOCK' AND (tblOper.OpNam)<>'SCRAP') AND ((IIf([zstblwhatif].[display],[zstblwhatif].[name],'skip'))<>'skip'))";

    public static readonly int[] myArray = new int[] { 9, 26, 3, 2, 5, 7, 10, 22, 21, 25, 11, 13, 12, 24, 23, 8, 16, 17, 18, 19, 14, 15 };

    [WebMethod(EnableSession = true)]
    public static List<string[]> SetGoogleData()
    {

        return Query.databaseQueryArray(myQuery, myArray);

    }

}                   

// 0 Equipment = 9
// 1 Scenario = 26
// 2 ProductName = 3
// 3 ProductGroup = 2
// 4 OperationName = 5
// 5 OperationNum = 7
// 6 E_Setup = 10
// 7 WFL = 22
// 8 WFE = 21
// 9 WFLot = 25
// 10 E_Run = 11
// 11 L_Run = 13
// 12 L_Setup = 12
// 13 T_Run = 24
// 14 T_Setup = 23
// 15 Labor = 8
// 16 ThousandVisits = 16
// 17 GoodVisists = 17
// 18 Setups = 18
// 19 LotSize = 19
// 20 OperationMCT = 14
// 21 WIP = 15
