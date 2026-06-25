<%@ Page Language="C#" AutoEventWireup="true" CodeFile="debug-search.aspx.cs" Inherits="debug_search" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <title>搜索调试</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        table { border-collapse: collapse; width: 100%; margin-top: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        .highlight { background-color: #ffffcc; }
        .hidden { background-color: #ffebee; }
        .visible { background-color: #e8f5e9; }
    </style>
</head>
<body>
    <h1>搜索调试页面</h1>
    
    <div>
        <label>搜索关键词：</label>
        <input type="text" id="searchKey" placeholder="输入型号如 CC1206KKX5R9BB106" />
        <button onclick="doSearch()">查询</button>
    </div>

    <div id="results"></div>

    <script>
        function doSearch() {
            var key = document.getElementById('searchKey').value.trim();
            window.location.href = 'debug-search.aspx?q=' + encodeURIComponent(key);
        }
    </script>
</body>
</html>
