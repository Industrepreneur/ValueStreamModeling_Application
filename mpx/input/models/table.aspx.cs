using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class general : DbPage {


    public const short YES_NO_TYPE = 1;
    public const short BYTE_TYPE = 2;
    public const short INTEGER_TYPE = 3;
    public const short int_TYPE = 4;
    public const short CURRENCY_TYPE = 5;
    public const short SINGLE_TYPE = 6;
    public const short DOUBLE_TYPE = 7;
    public const short DATE_TIME_TYPE = 8;
    public const short TEXT_TYPE = 10;
    public const short MEMO_TYPE = 12;
    public const short name_type = 13;

    ClassE classE1_1;

    public general() {
        PAGENAME = "/input/models/table.aspx";

    }

    protected void Page_Load(object sender, EventArgs e) {
        base.Page_Load(sender, e);

        if (!Page.IsPostBack) {
            title1.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
           
            //comment.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            dtu.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            mcttu.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            optu.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            cv2.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            cv1.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            utlimit.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            laborcv.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            equipcv.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            partcv.Attributes.Add("onkeydown", "return (event.keyCode!=13);");

            try {
                ReadData();
            } catch (Exception) {
                Master.ShowErrorMessage("MPX internal error has occured.");

            }
        }
    }

 

    protected void SaveTitle(object sender, EventArgs e) {
        try {
            SaveTitle();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void PrepareClassE(string selectQueryString, ref ADODB.Recordset recgen) {
        try {
            DbUse.open_ado_rec(classE1_1.globaldb, ref recgen, selectQueryString);

            classE1_1.globTNameE = "General Data";
            classE1_1.globTNameA = "tblgeneral";
            classE1_1.globrecid = (int)recgen.Fields["Generalid"].Value;

            classE1_1.globDType = TEXT_TYPE;
            classE1_1.globFNameE = "  -  ";
            classE1_1.globFNameA = "t";
            classE1_1.globOVal = "1";
            classE1_1.globNVal = "0";
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void SaveTitle() {
        string selectQueryString = "Select * from tblgeneral;";
        ADODB.Recordset recgen = null;
        int fstatus;

        PrepareClassE(selectQueryString, ref recgen);

        string newValue = title1.Value;


        newValue = MyUtilities.clean(newValue);
        newValue = newValue.Trim();
        if (newValue != (string)recgen.Fields["title"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = TEXT_TYPE;
                classE1_1.globFNameE = "Title of Model";
                classE1_1.globFNameA = "title";
                classE1_1.globOVal = (string)recgen.Fields["title"].Value;
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.title  = '" + newValue + "';");
        }

        newValue = comment.Value;
        newValue = MyUtilities.clean(newValue);
        newValue = newValue.Trim();
        if (selectQueryString != (string)recgen.Fields["com"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = TEXT_TYPE;
                classE1_1.globFNameE = "Comment";
                classE1_1.globFNameA = "com";
                classE1_1.globOVal = (string)recgen.Fields["com"].Value;
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.com  = '" + newValue + "';");
        }
    }

    protected void SaveUnits() {
        string selectQueryString = "Select * from tblgeneral;";
        ADODB.Recordset recgen = null;
        int fstatus;

        PrepareClassE(selectQueryString, ref recgen);

        string newValue = dtu.Text;

        newValue = MyUtilities.clean(newValue).ToUpper();
        newValue = newValue.Trim();
        if (newValue != (string)recgen.Fields["tufor"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = TEXT_TYPE;
                classE1_1.globFNameE = "Forcast Period Time Unit";
                classE1_1.globFNameA = "TUFOR";
                classE1_1.globOVal = (string)recgen.Fields["tufor"].Value;
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.tufor  = '" + newValue + "';");
        }

        newValue = mcttu.Text;

        newValue = MyUtilities.clean(newValue);
        newValue = newValue.Trim();
        if (newValue != (string)recgen.Fields["tult"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = TEXT_TYPE;
                classE1_1.globFNameE = "MCT Time Unit";
                classE1_1.globFNameA = "TULT";
                classE1_1.globOVal = (string)recgen.Fields["tult"].Value;
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.tult  = '" + newValue + "';");
        }
        newValue = optu.Text;
        newValue = MyUtilities.clean(newValue);
        newValue = newValue.Trim();
        if (newValue != (string)recgen.Fields["tult"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = TEXT_TYPE;
                classE1_1.globFNameE = "Operation Time Unit";
                classE1_1.globFNameA = "TUprod";
                classE1_1.globOVal = (string)recgen.Fields["tuprod"].Value;
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tuprod  = '" + newValue + "';");
        }

    }

    protected void SaveUnits(object sender, EventArgs e) {
        try {
            SaveUnits();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void SaveConversions(object sender, EventArgs e) {
        try {
            SaveConversions();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");

        }
    }

    protected void SaveConversions() {
        string selectQueryString = "Select * from tblgeneral;";
        ADODB.Recordset recgen = null;
        int fstatus;

        PrepareClassE(selectQueryString, ref recgen);

        string newValue = cv1.Text;
        newValue = MyUtilities.clean(newValue);
        string myHelp = recgen.Fields["rtu1b"].Value.ToString();
        string gregHelp = selectQueryString;
        if (Convert.ToSingle(newValue) != (float)recgen.Fields["rtu1b"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = SINGLE_TYPE;
                classE1_1.globFNameE = "Oper Time / Manuf. Critical-path Time";
                classE1_1.globFNameA = "rtu1b";
                classE1_1.globOVal = Convert.ToString((float)recgen.Fields["rtu1b"].Value);
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set rtu1b  = " + newValue + ";");
        }

        newValue = cv2.Text;
        newValue = MyUtilities.clean(newValue);
        if (Convert.ToSingle(newValue) != (float)recgen.Fields["rtu1c"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = SINGLE_TYPE;
                classE1_1.globFNameE = "Manuf. Critical-path Time / Forecast Period";
                classE1_1.globFNameA = "rtu1c";
                classE1_1.globOVal = Convert.ToString((float)recgen.Fields["rtu1c"].Value);
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set rtu1c  = " + newValue + ";");
        }
    }

    protected void SaveAdvanced(object sender, EventArgs e) {
        try {
            SaveAdvanced();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
    }

    protected void SaveAdvanced() {
        string selectQueryString = "Select * from tblgeneral;";
        ADODB.Recordset recgen = null;
        int fstatus;

        PrepareClassE(selectQueryString, ref recgen);

        string newValue = utlimit.Text;

        newValue = MyUtilities.clean(newValue);
        if (Convert.ToSingle(newValue) != (float)recgen.Fields["utlimit"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = SINGLE_TYPE;
                classE1_1.globFNameE = "Utilization Limit";
                classE1_1.globFNameA = "utlimit";
                classE1_1.globOVal = Convert.ToString((float)recgen.Fields["utlimit"].Value);
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set utlimit  = " + newValue + ";");
        }

        newValue = laborcv.Text;
        newValue = MyUtilities.clean(newValue);
        if (Convert.ToSingle(newValue) != (float)recgen.Fields["coef_v_labor"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = SINGLE_TYPE;
                classE1_1.globFNameE = "Labor Variability";
                classE1_1.globFNameA = "coef_v_labor";
                classE1_1.globOVal = Convert.ToString((float)recgen.Fields["coef_v_labor"].Value);
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.coef_v_labor  = " + newValue + ";");
        }

        newValue = equipcv.Text;
        newValue = MyUtilities.clean(newValue);
        if (Convert.ToSingle(newValue) != (float)recgen.Fields["coef_v_labor"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = SINGLE_TYPE;
                classE1_1.globFNameE = "Equipent Variability";
                classE1_1.globFNameA = "coef_v_equip";
                classE1_1.globOVal = Convert.ToString((float)recgen.Fields["coef_v_equip"].Value);
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.coef_v_equip  = " + newValue + ";");
        }

        newValue = partcv.Text;
        newValue = MyUtilities.clean(newValue);
        if (Convert.ToSingle(newValue) != (float)recgen.Fields["coef_v_parts"].Value) {
            if (classE1_1.glngwid != 0) {
                classE1_1.globDType = SINGLE_TYPE;
                classE1_1.globFNameE = "Product Start Variability";
                classE1_1.globFNameA = "coef_v_parts";
                classE1_1.globOVal = Convert.ToString((float)recgen.Fields["coef_v_parts"].Value);
                classE1_1.globNVal = newValue;
                fstatus = classE1_1.InsertAudit();
            }
            UpdateSql("update tblgeneral set tblgeneral.coef_v_parts  = " + newValue + ";");
        }
    }


    protected bool SaveData(HttpRequest request) {
        string selectQueryString = "Select * from tblgeneral;";
        ADODB.Recordset recgen = null;
        int fstatus;

        DbUse.open_ado_rec(classE1_1.globaldb, ref recgen, selectQueryString);



        classE1_1.globTNameE = "General Data";
        classE1_1.globTNameA = "tblgeneral";
        classE1_1.globrecid = (int)recgen.Fields["Generalid"].Value;

        classE1_1.globDType = TEXT_TYPE;
        classE1_1.globFNameE = "  -  ";
        classE1_1.globFNameA = "t";
        classE1_1.globOVal = "1";
        classE1_1.globNVal = "0";


        foreach (string query in request.Form) {
            string newValue = String.Format("{0}", request.Form[query]);


            newValue = MyUtilities.clean(newValue);

            if (query.IndexOf("title1") > 0) {
                newValue = MyUtilities.clean(newValue);
                newValue = newValue.Trim();
                if (newValue != (string)recgen.Fields["title"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = TEXT_TYPE;
                        classE1_1.globFNameE = "Title of Model";
                        classE1_1.globFNameA = "title";
                        classE1_1.globOVal = (string)recgen.Fields["title"].Value;
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.title  = '" + newValue + "';");
                }
            } else if (query.IndexOf("dtu") > -1) {
                newValue = MyUtilities.clean(newValue);
                newValue = newValue.Trim();
                if (newValue != (string)recgen.Fields["tufor"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = TEXT_TYPE;
                        classE1_1.globFNameE = "Forcast Period Time Unit";
                        classE1_1.globFNameA = "TUFOR";
                        classE1_1.globOVal = (string)recgen.Fields["tufor"].Value;
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.tufor  = '" + newValue + "';");
                }
            } else if (query.IndexOf("mcttu") > -1) {
                newValue = MyUtilities.clean(newValue);
                newValue = newValue.Trim();
                if (newValue != (string)recgen.Fields["tult"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = TEXT_TYPE;
                        classE1_1.globFNameE = "MCT Time Unit";
                        classE1_1.globFNameA = "TULT";
                        classE1_1.globOVal = (string)recgen.Fields["tult"].Value;
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.tult  = '" + newValue + "';");
                }
            } else if (query.IndexOf("optu") > -1) {
                newValue = MyUtilities.clean(newValue);
                newValue = newValue.Trim();
                if (newValue != (string)recgen.Fields["tult"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = TEXT_TYPE;
                        classE1_1.globFNameE = "Operation Time Unit";
                        classE1_1.globFNameA = "TUprod";
                        classE1_1.globOVal = (string)recgen.Fields["tuprod"].Value;
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tuprod  = '" + newValue + "';");
                }
            } else if (query.IndexOf("cv1") > -1) {
                newValue = MyUtilities.clean(newValue);
                string myHelp = recgen.Fields["rtu1b"].Value.ToString();
                string gregHelp = selectQueryString;
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["rtu1b"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "Oper Time / Manuf. Critical-path Time";
                        classE1_1.globFNameA = "rtu1b";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["rtu1b"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set rtu1b  = " + newValue + ";");
                }
            } else if (query.IndexOf("cv2") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["rtu1c"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "Manuf. Critical-path Time / Forecast Period";
                        classE1_1.globFNameA = "rtu1c";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["rtu1c"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set rtu1c  = " + newValue + ";");
                }
            } else if (query.IndexOf("utlimit") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["utlimit"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "Utilization Limit";
                        classE1_1.globFNameA = "utlimit";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["utlimit"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set utlimit  = " + newValue + ";");
                }
            } else if (query.IndexOf("laborcv") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["coef_v_labor"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "Labor Variability";
                        classE1_1.globFNameA = "coef_v_labor";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["coef_v_labor"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.coef_v_labor  = " + newValue + ";");
                }
            } else if (query.IndexOf("equipcv") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["coef_v_labor"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "Equipent Variability";
                        classE1_1.globFNameA = "coef_v_equip";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["coef_v_equip"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.coef_v_equip  = " + newValue + ";");
                }
            } else if (query.IndexOf("partcv") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["coef_v_parts"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "Product Start Variability";
                        classE1_1.globFNameA = "coef_v_parts";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["coef_v_parts"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.coef_v_parts  = " + newValue + ";");
                }
            } else if (query.IndexOf("g1") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["g1"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "General Variable #1";
                        classE1_1.globFNameA = "g1";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["g1"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.g1  = " + newValue + ";");
                }
            } else if (query.IndexOf("g2") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["g2"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "General Variable #2";
                        classE1_1.globFNameA = "g2";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["g2"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.g2  = " + newValue + ";");
                }
            } else if (query.IndexOf("g3") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["g3"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "General Variable #3";
                        classE1_1.globFNameA = "g3";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["g3"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.g3  = " + newValue + ";");
                }
            } else if (query.IndexOf("g4") > -1) {
                newValue = MyUtilities.clean(newValue);
                if (Convert.ToSingle(newValue) != (float)recgen.Fields["g4"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = SINGLE_TYPE;
                        classE1_1.globFNameE = "General Variable #4";
                        classE1_1.globFNameA = "g4";
                        classE1_1.globOVal = Convert.ToString((float)recgen.Fields["g4"].Value);
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.g4  = " + newValue + ";");
                }
            } else if (query.IndexOf("comment") > 0) {
                newValue = MyUtilities.clean(newValue);
                newValue = newValue.Trim();
                if (selectQueryString != (string)recgen.Fields["com"].Value) {
                    if (classE1_1.glngwid != 0) {
                        classE1_1.globDType = TEXT_TYPE;
                        classE1_1.globFNameE = "Comment";
                        classE1_1.globFNameA = "com";
                        classE1_1.globOVal = (string)recgen.Fields["com"].Value;
                        classE1_1.globNVal = newValue;
                        fstatus = classE1_1.InsertAudit();
                    }
                    UpdateSql("update tblgeneral set tblgeneral.com  = '" + newValue + "';");
                }
            }


        }
        return true;
    }

    protected void SaveAll(object sender, EventArgs e) {
        SaveTitle();
        SaveUnits();
        SaveConversions();
        SaveAdvanced();
        SetModelModified(true);
        Response.Redirect(Request.RawUrl);
    }

    protected void btnAdvanced_Click(object sender, EventArgs e) {
        if (col_7.Attributes["class"] == "collapse" || col_8.Attributes["class"] == "collapse" || col_9.Attributes["class"] == "collapse" || col_10.Attributes["class"] == "collapse" || col_7_txt.Attributes["class"] == "collapse" || col_8_txt.Attributes["class"] == "collapse" || col_9_txt.Attributes["class"] == "collapse" || col_10_txt.Attributes["class"] == "collapse")
        {
            col_7.Attributes["class"] = "expand";
            col_8.Attributes["class"] = "expand";
            col_9.Attributes["class"] = "expand";
            col_10.Attributes["class"] = "expand";
            col_7_txt.Attributes["class"] = "expand";
            col_8_txt.Attributes["class"] = "expand";
            col_9_txt.Attributes["class"] = "expand";
            col_10_txt.Attributes["class"] = "expand";
            lblAdvanced.Text = "<i class='fas fa-eye-slash fa-fw row-icon'></i><span id='spnAdvanced'>SHOW/HIDE</span>";
            lblAdvanced.ToolTip = "Hide Optional Parameters";
        }
        else
        {
            col_7.Attributes["class"] = "collapse";
            col_8.Attributes["class"] = "collapse";
            col_9.Attributes["class"] = "collapse";
            col_10.Attributes["class"] = "collapse";
            col_7_txt.Attributes["class"] = "collapse";
            col_8_txt.Attributes["class"] = "collapse";
            col_9_txt.Attributes["class"] = "collapse";
            col_10_txt.Attributes["class"] = "collapse";
            lblAdvanced.Text = "<i class='fas fa-eye-slash fa-fw-slash row-icon'></i><span id='spnAdvanced'>SHOW/HIDE</span>";
            lblAdvanced.ToolTip = "Show Optional Parameters";
        }
        //pnlAdvanced.Visible = !pnlAdvanced.Visible;
        //if (pnlAdvanced.Visible) {
        //    btnAdvanced.Text = "Hide Optional Parameters";
        //} else {
        //    btnAdvanced.Text = "Show Optional Parameters";
        //}

    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        try {
            classE1_1 = new ClassE(GetDirectory() + userDir);
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");
        }
        string sheet = "Cheat Sheet General Input Page";
        Master.SetHelpSheet(sheet + ".pdf", sheet);
    }
    protected void btnReset_Click(object sender, EventArgs e) {
        try {
        ReadData();
        } catch (Exception) {
            Master.ShowErrorMessage("MPX internal error has occured.");            
        }
    }

    protected void ReadData() {
        bool adoOpened = DbUse.open_ado(ref conn, GetDirectory() + userDir + MAIN_USER_DATABASE);
        string commandString = "SELECT " + TBL_GENERAL + ".* FROM(" + TBL_GENERAL + ");";
        bool adoRecOpened = DbUse.open_ado_rec(conn, ref  rec, commandString);

        try {
            if (!adoOpened || !adoRecOpened)
                throw new Exception("Error in opening database/dataset. The data cannot be displayed.");
            try {
                string title = (string)rec.Fields["Title"].Value;
                title1.Value = title;
                comment.Value = rec.Fields["com"].Value.ToString();
                dtu.Text = (string)rec.Fields["TUFor"].Value;
                mcttu.Text = (string)rec.Fields["tult"].Value;
                optu.Text = (string)rec.Fields["tuprod"].Value;

                cv1.Text = rec.Fields["RTU1b"].Value.ToString();
                cv2.Text = rec.Fields["RTU1c"].Value.ToString();

                utlimit.Text = rec.Fields["Utlimit"].Value.ToString();
                laborcv.Text = rec.Fields["coef_v_labor"].Value.ToString();
                equipcv.Text = rec.Fields["coef_v_equip"].Value.ToString();
                partcv.Text = rec.Fields["coef_v_parts"].Value.ToString();
            } catch (Exception) {
                Master.ShowErrorMessage("An error has occured while reading the data from the database. Some data might not display correctly.");
            }
        } catch (Exception ex) {
            Master.ShowErrorMessage(ex.Message);
        }
        DbUse.CloseAdo(conn);
        DbUse.CloseAdoRec(rec);

    }


}