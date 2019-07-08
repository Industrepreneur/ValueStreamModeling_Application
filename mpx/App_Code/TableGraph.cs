using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;

/// <summary>
/// Output page with table and a graph
/// </summary>
public abstract class TableGraph : SortablePage {

    protected string[] FIELDS; // column names in the table

    protected string[] HEADERS; // headers for the columns in the table

    protected string[] HEADERS2; // headers for the two header row

    protected string tableQueryString;

    protected string graphQueryString;

    protected bool graphSet = false;

    protected const float GRAPH_MIN_VALUE_FRAC = 0.01f;

    protected GridView grid;

    protected Chart chart;

    protected string[] DESC;

    protected int[] FIELD_OFFSETS;

    protected int graphType;

    protected bool isUtilizationGraph = false;

    protected double minVisibleValue = 0;

    protected bool wantTwoHeaders = false;

    protected Panel pnlCopyTable;

    public class GraphType {
        public const int STACKED_COLUMN = 0;
        public const int BAR = 1;
    }

    protected bool[] UNITS;

    // was green 85cf37

    // colors for each series
    protected string[] COLORS = { "#546787", "#85CF37", "#C0200A", "#FFFF46", "#FFADFF", "#B2B2B2" };
    // yellow, green, dark red/brown
    protected string[] COLORS_LABOR_UT = { "#FFFF33", "#00BC00", "#800000" };

    // green, orange
    protected string[] COLORS_EQ_WAITING_LABOR = { "#00BC00", "#FF7519" };

    // yellow, green, orange, dark red/brown
    protected string[] COLORS_EQUIP_UT = { "#FFFF33", "#00BC00", "#FF7519", "#800000" };

    // green, red
    protected string[] COLORS_EQUIP_WIP = { "#00BC00", "#FF1919" };

    // green, blue, red, black
    protected string[] COLORS_PROD_LEVEL = { "#00BC00", "#3C79D1", "#FF1919", "#323232" };

    // cyan, red, orange, yellow, green, magenta
    public static string[] COLORS_MCT = { "#6CFFFF", "#FF1919", "#FF7519", "#FFFF33", "#00BC00", "#E62E8A" };

    // green
    protected string[] COLORS_PROD_WIP = { "#85cf37" };
    //#E0400A - orange/red; #A34719 - brown color

    protected string sortedTableNameGraph;

    /************************/
    //stuff for the copy table panel
    protected TextBox txtCopyTable;
    protected RadioButton rdbtnTableWithHeaders;
    protected RadioButton rdbtnTableWithoutHeaders;
    protected CheckBox boxCheckAll;
    protected AjaxControlToolkit.ModalPopupExtender extenderCopy;

    protected string TableStringHeaders {
        get {
            if (ViewState["TableStringHeaders"] == null) {
                ViewState["TableStringHeaders"] = "";
            }
            return (string)ViewState["TableStringHeaders"];
        }
        set { ViewState["TableStringHeaders"] = value; }
    }

    protected string TableString {
        get {
            if (ViewState["TableString"] == null) {
                ViewState["TableString"] = "";
            }
            return (string)ViewState["TableString"];
        }
        set { ViewState["TableString"] = value; }
    }
    /********************************************************/

    protected void SetTableData() {
        string order;
        try {
            order = GetOrderBy(sortedTableName);
            if (order == null || order.Trim().Equals("")) {
                order = defaultSortString;
            }
        } catch (Exception) {
            order = defaultSortString;
        }


        tableQueryString += " " + order + ";";
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(tableQueryString, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                

                DataView dv = dt.DefaultView;
                grid.DataSource = dv;
                grid.DataBind();
                LoadTableForCopy(dt);
                connec.Close();
                if (dt.Rows.Count == 0) {
                    //Master.ShowErrorMessage("There are no results. There are either some errors in the model or the results have not yet been calculated.");
                }


            } catch (Exception ex) {
                logFiles.ErrorLog(ex);
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                if (!TablesLinked()) {
                    Master.ShowErrorMessage("An error has occured. Current model '" + Master.GetCurrentModel() + "' is not loaded properly because some tables are missing. Please go to the models page and load the model again.");
                } else {
                    Master.ShowErrorMessage("An error has occured and the table cannot be displayed.");
                }
            }
        }
    }

    private void LoadTableForCopy(DataTable dt) {
        string copiedTable = "";
        TableString = "";
        TableStringHeaders = "";

        for (int i = 1; i < HEADERS.Length; i++) {
            string header = HEADERS[i];
            if (wantTwoHeaders && HEADERS2 != null) {
                header = HEADERS2[i];
            }
            copiedTable += header;
            TableStringHeaders += header;

            if (i != HEADERS.Length - 1) {
                copiedTable += "\t"; // tab for next cell
                TableStringHeaders += "\t";
            } else {
                copiedTable += "\r\n"; // end of table line
                TableStringHeaders += "\r\n";
            }
        }

        foreach (DataRow row in dt.Rows) {
            for (int i = 1; i < FIELDS.Length; i++) {

                string field = MyUtilities.clean(FIELDS[i], '[');
                field = MyUtilities.clean(field, ']');
                copiedTable += row[field].ToString();
                TableString += row[field].ToString();
                if (i != FIELDS.Length - 1) {
                    copiedTable += "\t"; // tab for next cell
                    TableString += "\t";
                } else {
                    copiedTable += "\r\n"; // end of table line
                    TableString += "\r\n";
                }
            }
        }
        if (rdbtnTableWithHeaders.Checked) {
            txtCopyTable.Text = TableStringHeaders + TableString;
        } else {
            txtCopyTable.Text = TableString;
        }

    }

    protected void grid_RowDataBound(object o, GridViewRowEventArgs e) {
        if (e.Row.RowType == DataControlRowType.DataRow) {
            foreach (TableCell cell in e.Row.Cells) {
                cell.DataBind();
                try {

                    foreach (Control control in cell.Controls) {
                        if (control is Label) {
                            Label lbl = control as Label;
                            //double num = Double.Parse(lbl.Text); 
                            //cell.HorizontalAlign = HorizontalAlign.Right;
                            lbl.Attributes["class"] += "padding";
                            break;
                        }
                    }

                } catch (Exception) {
                    
                }
            }
        }
    }

    protected virtual void SetGraphData(string xTitle, string yTitle, int columnSeries) {
        if (!graphSet) {
            graphSet = true;
            string order;
            try {
                if (sortedTableNameGraph != null) {
                    order = GetOrderBy(sortedTableNameGraph);
                } else {
                    order = GetOrderBy(sortedTableName);
                }
                if (order == null || order.Trim().Equals("")) {
                    order = defaultSortString;
                }
            } catch (Exception) {
                order = defaultSortString;
            }

            graphQueryString += " " + order + ";";

            chart = getChart();
            chart.ImageStorageMode = ImageStorageMode.UseImageLocation;
            string simpleUserdir = MyUtilities.clean(userDir, '\\');
            chart.ImageLocation = "~/" + DbPage.BROWSER_DIR + "/" + simpleUserdir + "/Graphs/Chart_#SEQ(500,3)";

            //gwwd added   remove old charts !!!
            //string chartloc;
            //chartloc = "~/" + DbPage.BROWSER_DIR + "/" + simpleUserdir + "/Graphs/Chart_*.*";
            //File.Delete(chartloc);

            chart.CssClass = "chart";
            // set legend
            Legend legend = new Legend();
            legend.LegendStyle = LegendStyle.Row;
            legend.Alignment = System.Drawing.StringAlignment.Center;
            legend.Docking = Docking.Top;
            legend.IsTextAutoFit = true;
            legend.ItemColumnSpacing = 300;
            chart.Legends.Add(legend);
            //chart.Width = 600;
            // format axis (titles and labels)
            ChartArea chartArea = chart.ChartAreas[0];
            chartArea.AxisY.Title = yTitle;
            chartArea.AxisX.Title = xTitle;
            chartArea.AxisY.LabelStyle.Font = new Font("Trebuchet MS", 10.25F, FontStyle.Bold);


            chartArea.AxisX.LabelStyle.Font = chartArea.AxisY.LabelStyle.Font;
            chartArea.AxisX.LabelStyle.Font = new Font("Trebuchet MS", 10.25F, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Trebuchet MS", 15.25F, FontStyle.Bold);
            chartArea.AxisX.TitleFont = chartArea.AxisY.TitleFont;
            chartArea.AxisX.TextOrientation = TextOrientation.Auto;
            chartArea.AxisX.MajorGrid.Enabled = false;
            //chartArea.BorderWidth = 1;
            //chartArea.BorderColor = Color.Black;
            //chartArea.BorderDashStyle = ChartDashStyle.Solid;

            switch (graphType) {
                case GraphType.STACKED_COLUMN:
                    if (FIELD_OFFSETS != null) {
                        FillStackedColumnGraph(columnSeries, FIELD_OFFSETS[0], FIELD_OFFSETS[1]);
                    } else {
                        FillStackedColumngGraph(columnSeries);
                    }
                    break;
                case GraphType.BAR:
                    FillBarGraph();
                    break;
                default:
                    break;
            }
            if (isUtilizationGraph) {
                //PlaceUtilizationLineAndTicks();
            }
            //chart.Visible = false;
        }


    }

    protected void xxPlaceUtilizationLineAndTicks() {
        // add utilization limit line
        ChartArea chartArea = chart.ChartAreas[0];
        StripLine line = new StripLine();
        line.Interval = 0;
        line.IntervalOffset = 90;  //  gwwd 6-26-17  was 95
        line.TextAlignment = StringAlignment.Near;
        line.TextLineAlignment = StringAlignment.Far;
        line.TextOrientation = TextOrientation.Auto;
        line.Font = new Font("Trebuchet MS", 11.25F, FontStyle.Bold);
        line.ForeColor = Color.Red;
        line.StripWidth = 0;
        line.BorderColor = Color.Red;
        line.BorderWidth = 3;
        line.Text.PadLeft(50);
        line.Text = "Utilization limit: 90%";  //  gwwd 6-26-17  was 95 
        chartArea.AxisY.StripLines.Add(line);

        chartArea.AxisY.MajorTickMark.Interval = 20;
        chartArea.AxisY.MajorTickMark.Enabled = true;

        chartArea.AxisY.Interval = 20;
        chartArea.AxisY.MinorTickMark.Interval = 5;
        chartArea.AxisY.MinorTickMark.Enabled = true;
    }

    protected virtual void FillStackedColumnGraph(int columnSeries, int fieldOffset, int headerOffset) {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(graphQueryString, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                // fill data table
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0) {
                    int minWidth = COLORS.Length * 160;
                    minWidth = Math.Max(minWidth, 450);
                    int chartWidth;
                    chart.Height = new Unit(600, UnitType.Pixel);
                    if (dt.Rows.Count > 10) {
                        chartWidth = dt.Rows.Count * 105;
                    } else if (dt.Rows.Count > 5) {
                        chartWidth = dt.Rows.Count * 120;
                    } else {
                        chartWidth = dt.Rows.Count * 150;

                    }
                    chartWidth = Math.Max(minWidth, chartWidth);
                    chart.Width = new Unit(chartWidth, UnitType.Pixel);
                }

                float[] sumStacks = new float[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++) {
                    sumStacks[i] = 0;
                }
                DrawColumns(dt, columnSeries, sumStacks, fieldOffset, headerOffset);
                //if (!isUtilizationGraph) {
                double maxValue = MyUtilities.GetMaxFloat(sumStacks);
                SetColumnGraphTicks(maxValue);
                //}

                connec.Close();
            } catch (Exception ex) {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage();
            }
        }
    }

    protected virtual float GetMinVisibleValue(DataTable dt, int columnSeries, float[] sumStacks, int fieldOffset) {
        for (int i = 1; i < columnSeries; i++) {
            for (int j = 0; j < dt.Rows.Count; j++) {
                float value = float.Parse(dt.Rows[j][FIELDS[i + fieldOffset]].ToString());
                sumStacks[j] += value;
            }
        }
        float maxValue = MyUtilities.GetMaxFloat(sumStacks);
        if (isUtilizationGraph) {
            maxValue = Math.Max(maxValue, 100);
        }
        float maxY = ClassB.FULL_SCALE(maxValue);
        int roundMaxY = (int)maxY;
        float maxYend = roundMaxY;

        return maxYend * GRAPH_MIN_VALUE_FRAC;

    }

    protected void FillBarGraph() {
        OleDbConnection connec = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + GetDirectory() + userDir + MAIN_USER_DATABASE + ";");
        OleDbCommand cmd = new OleDbCommand(graphQueryString, connec);
        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
        {
            try {
                // fill data table
                connec.Open();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0) {
                    chart.Height = new Unit(600, UnitType.Pixel);
                    int width = dt.Rows.Count * 120;
                    width = Math.Max(width, 400);
                    chart.Width = new Unit(width, UnitType.Pixel);
                }


                float[] barHeights = new float[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++) {
                    barHeights[i] = 0;
                }
                for (int j = 0; j < dt.Rows.Count; j++) {
                    barHeights[j] = float.Parse(dt.Rows[j][FIELDS[3]].ToString());
                }
                float maxValue = MyUtilities.GetMaxFloat(barHeights);
                float maxY = ClassB.FULL_SCALE(maxValue);
                int roundMaxY = (int)maxY;
                float maxYend = roundMaxY;

                float minVisibleValue = maxYend * GRAPH_MIN_VALUE_FRAC;


                Series series = new Series();
                series.ChartType = SeriesChartType.Column;
                series.YValueType = ChartValueType.Double;
                for (int j = 0; j < dt.Rows.Count; j++) {
                    if (barHeights[j] > 0 && barHeights[j] < minVisibleValue) {
                        barHeights[j] = minVisibleValue;
                    }
                    series.Points.AddY(barHeights[j]);
                    string dataCase = dt.Rows[j][DESC[0]].ToString();
                    dataCase = GetGraphColHeader(dataCase);
                    string desc = dt.Rows[j][DESC[1]].ToString();
                    desc = GetGraphColHeader(desc);
                    series.Points[j].AxisLabel = dataCase + "\n" + desc;


                }
                series.IsVisibleInLegend = false;
                series.Name = HEADERS[3];
                series.Color = ColorTranslator.FromHtml(COLORS[0]);
                chart.Series.Add(series);
                SetColumnGraphTicks(maxValue);
                chart.ChartAreas[0].AxisX.Interval = 1;
                connec.Close();
            } catch (Exception) {
                try {
                    connec.Close();
                    connec = null;
                } catch { }
                Master.ShowErrorMessage();
            }
        }
    }

    private int NUM_CHARACTERS_IN_ROW = 10;

    protected string GetGraphColHeader(string header) {
        return GetGraphColHeader(header, 3);
    }

    protected string GetGraphColHeader(string header, int maxRows) {
        string divHeader = "";
        int i = 0;
        while (header.Length > 0 && i < maxRows) {
            if (divHeader.Length > 0) {
                divHeader += "\n";
            }
            int locLength = Math.Min(header.Length, NUM_CHARACTERS_IN_ROW);
            divHeader += header.Substring(0, locLength);
            if (locLength < header.Length) {
                header = header.Substring(locLength);
            } else {
                header = "";
            }
        }
        return divHeader;
    }


    protected void SetColumnGraphTicks(double maxValue) {
        ChartArea chartArea = chart.ChartAreas[0];

        if (isUtilizationGraph) {
            if (maxValue < 100) {
                maxValue = 100;
            }
            StripLine line = new StripLine();
            line.Interval = 0;
            int maxut_val;
            ClassB Bcla_1 = new ClassB(GetDirectory() + userDir);
            maxut_val = (int)Math.Round(Convert.ToSingle(Bcla_1.get_utlimit()));
            line.IntervalOffset = maxut_val; //  gwwd 6-26-17  was 95 
            line.TextAlignment = StringAlignment.Near;
            line.TextLineAlignment = StringAlignment.Far;
            line.TextOrientation = TextOrientation.Auto;
            line.Font = new Font("Trebuchet MS", 11.25F, FontStyle.Bold);
            line.ForeColor = Color.Red;
            line.StripWidth = 0;
            line.BorderColor = Color.Red;
            line.BorderWidth = 3;
            line.Text.PadLeft(50);
            line.Text = "Utilization limit: " + maxut_val + "%";  //  gwwd 6-26-17  was 95 
            chartArea.AxisY.StripLines.Add(line);
        }

        if (maxValue == 0.0) {

            chartArea.AxisY.MajorTickMark.Interval = 50;
            chartArea.AxisY.MajorTickMark.Enabled = true;
            chartArea.AxisY.Interval = 50;
            chartArea.AxisY.MinorTickMark.Interval = 10;
            chartArea.AxisY.MinorTickMark.Enabled = true;
        } else {
            /*string val = maxValue + ""; 
            int numCiphers = val.Length;
            int order;
            if (val.IndexOf('.') > -1) {
               order  = ((int) (maxValue*Math.Pow(10, numCiphers - val.IndexOf('.') - 1)) / ((int)Math.Pow(10, numCiphers - 2)));
            } else {
               order  = ((int)Math.Round(maxValue)) / ((int)Math.Pow(10, numCiphers - 1));
            }
            order++;
            if (order % 2 == 1) {
                order++;
            }
            if (order % 4 == 2) {
                order += 2;
            }

            if (val.IndexOf('.') > -1) {
                chartArea.AxisY.MajorTickMark.Interval = (((double)order) / 8) * ((int)Math.Pow(10, val.IndexOf('.')-1));
            } else {
                chartArea.AxisY.MajorTickMark.Interval = (order / 4) * ((int)Math.Pow(10, numCiphers - 1));
            }*/

            chartArea.AxisY.MajorTickMark.Enabled = true;
            chartArea.AxisY.MinorTickMark.Enabled = true;

            double maxY = ClassB.FULL_SCALE((float)maxValue);
            int roundMaxY = (int)maxY;
            if (roundMaxY % 5 == 0) {
                chartArea.AxisY.MajorTickMark.Interval = maxY / 5;
            } else if (roundMaxY % 4 == 0) {
                chartArea.AxisY.MajorTickMark.Interval = maxY / 4;
            } else if (roundMaxY % 3 == 0) {
                chartArea.AxisY.MajorTickMark.Interval = maxY / 3;
            } else {
                chartArea.AxisY.MajorTickMark.Interval = 1;
            }
            chartArea.AxisY.Maximum = maxY;
            int majorInt = (int)chartArea.AxisY.MajorTickMark.Interval;
            string majInt = majorInt + "";
            int firstCipher = int.Parse(majInt.Substring(0, 1));
            if (firstCipher % 4 == 0) {
                chartArea.AxisY.MinorTickMark.Interval = chartArea.AxisY.MajorTickMark.Interval / 4;
            } else if (firstCipher % 3 == 0) {
                chartArea.AxisY.MinorTickMark.Interval = chartArea.AxisY.MajorTickMark.Interval / 3;
            } else {
                chartArea.AxisY.MinorTickMark.Interval = chartArea.AxisY.MajorTickMark.Interval / 5;
            }
            chartArea.AxisY.Interval = chartArea.AxisY.MajorTickMark.Interval;
            //chartArea.AxisY.Interval = chartArea.AxisY.MajorTickMark.Interval;
            //chartArea.AxisY.MinorTickMark.Interval = chartArea.AxisY.MajorTickMark.Interval / 5;




        }
    }

    protected virtual void DrawColumns(DataTable dt, int columnSeries, float[] sumStacks, int fieldOffset, int headerOffset) {
        float minVisibleValue = GetMinVisibleValue(dt, columnSeries, sumStacks, fieldOffset);

        for (int j = 0; j < dt.Rows.Count; j++) {
            sumStacks[j] = 0;
        }

        // add series - one color = one series
        for (int i = 1; i < columnSeries; i++) {
            Series series = new Series();
            series.ChartType = SeriesChartType.StackedColumn;
            series.YValueType = ChartValueType.Double;
            float[] stackValues = new float[dt.Rows.Count];
            for (int j = 0; j< dt.Rows.Count; j++) {
                stackValues[j] = float.Parse(dt.Rows[j][FIELDS[i + fieldOffset]].ToString());
            }
            AdjustStackedValues(stackValues, minVisibleValue);
            for (int j = 0; j < dt.Rows.Count; j++) {

                //double value = Double.Parse(dt.Rows[j][FIELDS[i + fieldOffset]].ToString());
                //if (value > 0 && value < minVisibleValue) {
                //    value = minVisibleValue;
                //}
                sumStacks[j] += stackValues[j];
                series.Points.AddY(stackValues[j]);
                if (i == 1) {
                    string dataCase = dt.Rows[j][DESC[0]].ToString();
                    dataCase = GetGraphColHeader(dataCase);
                    string desc = dt.Rows[j][DESC[1]].ToString();
                    desc = GetGraphColHeader(desc);
                    series.Points[j].AxisLabel = dataCase + "\n" + desc;

                }
            }
            series.IsVisibleInLegend = true;
            series.Name = HEADERS[i + headerOffset].Replace("<br/>", " ");
            series.Color = ColorTranslator.FromHtml(COLORS[i - 1]);
            chart.Series.Add(series);

        }
        chart.ChartAreas[0].AxisX.Interval = 1;

    }

    protected void DrawColumns(DataTable dt, int columnSeries, float[] sumStacks) {
        DrawColumns(dt, columnSeries, sumStacks, 0, 0);
    }

    protected void FillStackedColumngGraph(int columnSeries) {
        FillStackedColumnGraph(columnSeries, 0, 0);
    }

    protected virtual void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grid = getGridView();
        grid.PageIndex = e.NewPageIndex;
        RefreshData();
    }



    // generate columns in the table grid control
    protected virtual void InitializeComponent() {
        grid = getGridView();
        if (grid != null) {
            grid.AutoGenerateColumns = false;
            grid.DataKeyNames = new string[] { FIELDS[0] };
            grid.AllowPaging = false;
            grid.PageIndexChanging += grid_PageIndexChanging;
            grid.RowDataBound += grid_RowDataBound;
            grid.AllowSorting = false;
            //grid.HeaderStyle.BackColor = ColorTranslator.FromHtml("#ffa500");
            //grid.RowStyle.CssClass = "datatable-rowstyle";
            //grid.AlternatingRowStyle.BackColor = Color.White;
            grid.ShowHeaderWhenEmpty = true;
            grid.EmptyDataText = "There are no data records to display.";
            //grid.BorderWidth = 3;
            //grid.BorderColor = Color.Black;
            for (int i = 1; i < FIELDS.Length; i++) {
                TemplateField template = new TemplateField();
                SimpleGridViewTemplate itemTemplate = new SimpleGridViewTemplate(ListItemType.Item, FIELDS[i]);
                template.ItemTemplate = itemTemplate;
                template.HeaderText = HEADERS[i];
                grid.Columns.Add(template);
            }
            PlacePnlCopyTable();
        }
    }

    /*********************************************/
    // stuff for table copy
    private void PlacePnlCopyTable() {
        Control buttondiv = GetSortButtonContainer();
        //NEED TO WRAP THIS IN LABEL FOR ICON/BUTTON COMBO, ASSIGN CSS CLASS
        Button btnCopyTable = new Button();
        btnCopyTable.Text = "Copy Table";  //  size ??
        //btnCopyTable.Height = 26;
        btnCopyTable.ID = PageControls.BTN_COPY_TABLE;
        btnCopyTable.ID = PageControls.BTN_COPY_TABLE;
        buttondiv.Controls.Add(btnCopyTable);

        Panel copyTableContainer = GetCopyTableContainer();
        if (copyTableContainer != null) {
            pnlCopyTable = PageControls.generateCopyPanel();

            copyTableContainer.Controls.Add(pnlCopyTable);
            rdbtnTableWithHeaders = pnlCopyTable.FindControl(PageControls.RDBTN_WITH_HEADER) as RadioButton;
            rdbtnTableWithoutHeaders = pnlCopyTable.FindControl(PageControls.RDBTN_WITHOUT_HEADER) as RadioButton;
            rdbtnTableWithHeaders.CheckedChanged += new EventHandler(rdbtnTable_CheckedChanged);
            rdbtnTableWithoutHeaders.CheckedChanged += new EventHandler(rdbtnTable_CheckedChanged);

            // these two lines don't work - won't cause correct postback
            //rdbtnTableWithHeaders.Attributes.Add("onClick", "HidePopup('" + PageControls.COPY_BEHAVIOR + "');  __doPostBack(this.id,'');");
            //rdbtnTableWithoutHeaders.Attributes.Add("onClick", "HidePopup('" + PageControls.COPY_BEHAVIOR + "');  __doPostBack(radioButton.id,'');");

            boxCheckAll = pnlCopyTable.FindControl(PageControls.CHECK_SELECT_ALL) as CheckBox;
            txtCopyTable = pnlCopyTable.FindControl(PageControls.INPUT_COPY_TABLE) as TextBox;
            boxCheckAll.Attributes.Add("onclick", "selectTable('" + txtCopyTable.ClientID + "', '" + boxCheckAll.ClientID + "')");
            Button btnCopyDone = pnlCopyTable.FindControl(PageControls.BTN_DONE) as Button;
            btnCopyDone.Click += new EventHandler(btnCopyDone_Click);
            btnCopyDone.OnClientClick = "HidePopup('" + PageControls.COPY_BEHAVIOR + "'); return true;";
            extenderCopy = PageControls.generateCopyExtender();
            copyTableContainer.Controls.Add(extenderCopy);
        }
    }



    protected abstract Panel GetCopyTableContainer();

    protected void rdbtnTable_CheckedChanged(object sender, EventArgs e) {
        if (rdbtnTableWithHeaders.Checked) {
            txtCopyTable.Text = TableStringHeaders + TableString;
        } else {
            txtCopyTable.Text = TableString;
        }
        boxCheckAll.Checked = false;
        extenderCopy.Show();
    }

    protected void btnCopyDone_Click(object sender, EventArgs e) {
        boxCheckAll.Checked = false;
    }
    /*************************************************************************/


    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
        AddUnitsToHeaders();
        InitializeComponent();
        setGraphColors();
        if (grid != null && wantTwoHeaders) {
            grid.RowCreated += GridView_RowCreated;
        }
        AddWhatifDisplayLabel();
    }

    protected void AddUnitsToHeaders() {
        if (UNITS != null) {
            for (int i = 1; i < HEADERS.Length; i++) {
                if (UNITS[i]) {
                    HEADERS[i] += " " + GetUnit(FIELDS[i]);
                }
            }
        }
    }

    protected void InitializeUnits() {
        UNITS = new bool[HEADERS.Length];
        for (int i = 1; i < HEADERS.Length; i++) {
            UNITS[i] = false;
        }
    }

    protected virtual string GetUnit(string field) {
        string unit = "";
        {
            ClassB classB = new ClassB(GetDirectory() + userDir);
            try {
                if (field.ToLower().IndexOf("flowtime") > -1 || field.ToLower().IndexOf("lt") > -1) {
                    unit = classB.get_lead_time_unit();
                } else if (field.ToLower().IndexOf("hours") > -1) {
                    unit = classB.get_op_time_unit();
                }
                unit = "[" + unit + "]";
            } catch (Exception) { }
        }
        return unit;
    }

    protected override void RefreshData() {
        GridView gridView = getGridView();
        if (gridView != null) {
            SetTableData();
        }
        Chart chart = getChart();
        if (chart != null) {
            SetGraphData();
        }

    }

    protected abstract GridView getGridView();
    protected abstract Chart getChart();

    protected virtual void SetGraphData() {

    }

    protected void setGraphColors() {
        if (PAGENAME == "results_labor_table.aspx") {
            COLORS = COLORS_LABOR_UT;
        } else if (PAGENAME == "results_labor_graph2.aspx") {
            COLORS = COLORS_EQ_WAITING_LABOR;
        } else if (PAGENAME == "results_equip.aspx") {
            COLORS = COLORS_EQUIP_UT;
        } else if (PAGENAME == "results_equip_graph2.aspx") {
            COLORS = COLORS_EQUIP_WIP;
        } else if (PAGENAME == "results_prod_table.aspx") {
            COLORS = COLORS_PROD_LEVEL;
        } else if (PAGENAME == "results_prod_graph2.aspx") {
            COLORS = COLORS_MCT;
        } else if (PAGENAME == "results_prod_graph3.aspx") {
            COLORS = COLORS_PROD_WIP;
        }
    }

    //property for storing of information about merged columns
    protected MergedColumnsInfo info {
        get {
            if (ViewState["info"] == null)
                ViewState["info"] = new MergedColumnsInfo();
            return (MergedColumnsInfo)ViewState["info"];
        }
    }

    protected virtual void SetupMergedColumns() {

    }

    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e) {
        //call the method for custom rendering the columns headers 
        if (e.Row.RowType == DataControlRowType.Header)
            e.Row.SetRenderMethodDelegate(RenderHeader);
    }

    //method for rendering the columns headers 
    private void RenderHeader(HtmlTextWriter output, Control container) {
        for (int i = 0; i < container.Controls.Count; i++) {
            TableCell cell = (TableCell)container.Controls[i];

            //stretch non merged columns for two rows
            if (!info.MergedColumns.Contains(i)) {
                cell.Attributes["rowspan"] = "2";
                cell.RenderControl(output);
            } else //render merged columns common title
                if (info.StartColumns.Contains(i)) {
                    output.Write(string.Format("<th align='center' colspan='{0}'>{1}</th>",
                             info.StartColumns[i], info.Titles[i]));
                }
        }

        //close the first row 
        output.Write("</tr>");
        //set attributes for the second row

        grid.HeaderStyle.AddAttributesToRender(output);
        //start the second row
        output.RenderBeginTag("tr");

        //render the second row (only the merged columns)
        for (int i = 0; i < info.MergedColumns.Count; i++) {
            TableCell cell = (TableCell)container.Controls[info.MergedColumns[i]];
            cell.RenderControl(output);
        }
    }

    protected virtual void AddWhatifDisplayLabel() {
        Label lblWhatifDisplay = new Label();
        lblWhatifDisplay.ID = "lblWhatifDisplay";
        lblWhatifDisplay.Text = DbUse.MESSAGE_WHATIF_DISPLAY;
        lblWhatifDisplay.CssClass = "datatable";
        Panel container = GetSortPanelContainer();
        if (container != null) {
            container.Controls.Add(lblWhatifDisplay);
        }
    }

    protected void AdjustStackedValues(float[] barNumbers, float minVisibleValue) {
        float sumNew;
        float sum = 0;

        for (int i = 0; i < barNumbers.Length; i++) {
            sum += barNumbers[i];
        }

        bool done = false;
        while (!done) {
            sumNew = 0;
            done = true;
            for (int i = 0; i < barNumbers.Length; i++) {
                if (barNumbers[i] < minVisibleValue && barNumbers[i] > 0) {
                    barNumbers[i] = minVisibleValue;
                    done = false;
                }
                sumNew += barNumbers[i];
            } // end for 
            if (!done) {
                for (int i = 0; i < barNumbers.Length; i++) {
                    if (barNumbers[i] > minVisibleValue) {
                        barNumbers[i] = barNumbers[i] * sum / sumNew;     //   may put a number under the limit ...
                    } // end if
                }  //  end for
            } // end  if 

        }  //  end while

        return;
    }  // end sub

}