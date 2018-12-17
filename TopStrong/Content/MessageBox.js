function MessageBoxShow(title,content,w,h)
{
    if (title != null && title != "" && title != undefined)
    {
        title = '<div style="width:100%;height:30px;text-align:center;">'+title+'</div>';
    }

    if (content == null || content == undefined)
    {
        content = "";
    }

    if (w == null || w=="" || w == undefined)
    {
        w = "80%";
    }

    if (h == null || h=="" || h == undefined)
    {
        h = "200px";
    }

    $("body").append('<div id="mask" style="position: absolute; top: 0px; filter: alpha(opacity=60); background-color: #777; z-index: 1002; left: 0px;opacity:0.5; -moz-opacity:0.5;" onclick="hideMask(1)"></div>');
    showMask();
    $("body").append('<div id="msbx" style="width:' + w + ';position: absolute;z-index: 1003;height:' + h + ';background-color: white;top: 20px;left: 7.5%;box-shadow:0px 0px 8px #808080; border-radius:10px;padding: 10px;">' + title + content + '</div>');
}

function SpreadBox(title,content)
{
    var html = '<div id="spbox" style="position:fixed;bottom:0px;width:100%;height:200px;background-color:white;z-index:1003;display:none;">';
    html += '<div style="width:98%;margin:0 auto; height:30px;color:red;"><div style="float:left;width:70%;font-weight: bold;">' + title + '</div><div style="float:right;width:30%;text-align:right;"><img src="../Upload/UI/exit.png" style="width:15px;height:15px;" onclick="hideMask(2)"/></div></div>';
    html += content;
    html += '</div>';
    $("body").append('<div id="mask" style="position: absolute; top: 0px; filter: alpha(opacity=60); background-color: #777; z-index: 1002; left: 0px;opacity:0.5; -moz-opacity:0.5;" onclick="hideMask(2)"></div>');
    showMask();
    $("body").append(html);
    $("#spbox").slideToggle("slow");
}

function Alert(content,time)
{
    if (time == null || time == "" || time == undefined)
    {
        time = 3000;
    }
    var t = setTimeout("hideMask(1)", time);
    $("body").append('<div id="msbx" style="width:50%;position: absolute;z-index: 1003;background-color:rgba(0,0,0,0.8);top: 200px;left: 22%;box-shadow:0px 0px 8px #808080; border-radius:10px;padding: 10px;"><div style="width:100%;text-align:center;color:white;">' + content + '</div></div>');
}

//兼容火狐、IE8   
//显示遮罩层    
function showMask() {
    $("#mask").css("height", $(document).height());
    $("#mask").css("width", $(document).width());
    $("#mask").show();
}
//隐藏遮罩层  
function hideMask(tp) {

    $("#mask").remove();
    if (tp == 1)
    {
        $("#msbx").remove();
    } else if (tp == 2)
    {
        $("#spbox").remove();
    }
}