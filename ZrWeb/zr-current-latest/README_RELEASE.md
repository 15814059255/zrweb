# 阻容网 ZR.net.cn 交付说明

本目录是完整可运行的静态前端交付包，包含页面源码、公共样式、公共脚本、数据库基线 SQL 和全站开发文档。

## 直接预览

```bash
python3 -m http.server 8080
```

浏览器访问：

```text
http://localhost:8080/index.html
```

## 生产静态发布

把本目录内全部文件上传到 Nginx、Apache、对象存储静态站点或 CDN Pages 的站点根目录。

推荐首页：

```text
index.html
```

## 数据库初始化

```bash
mysql -u root -p < zr-database-schema.sql
```

默认数据库名：

```text
zr_platform
```

## 主要文件

- `index.html`：供需广场首页。
- `search.html`：搜索结果页。
- `supply-detail.html`：供应详情。
- `demand-detail.html`：需求详情。
- `merchant-workbench.html`：商家工作台。
- `buyer-workbench.html`：采购工作台。
- `zr-admin-console.html`：后台控制台。
- `zr-admin-ads.html`：全网广告位总汇。
- `assets/styles.css`：全站样式。
- `assets/app.js`：全站交互脚本。
- `zr-database-schema.sql`：数据库基线。
- `zr-development-doc.html`：完整开发文档。

## 开发文档

打开：

```text
zr-development-doc.html
```

文档内包含页面功能、业务流程、搜索规则、广告位、数据库表结构、完整 SQL、源码结构、后端接口建议、部署步骤和测试记录。

