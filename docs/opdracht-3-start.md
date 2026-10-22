# Opdracht 3 中文起步指南

这次作业的目标是为现有 portfolio 建立后端：用 ASP.NET Core Web API
接收请求，用 EF Core 操作 SQL Server，并用 Swagger UI 验证功能。
当前先完成项目和 Swagger 的启动，再逐步实现业务功能。

## 你现在在哪里

现有网站已经有 HTML、CSS 和 JavaScript：

- `projecten.js` 从 JavaScript 数组读取项目。
- `blog.html` 直接在 HTML 中保存博客内容。
- `weer.js` 调用外部天气 API，可以帮助你理解“请求 API 后读取 JSON”。
- `contact.js` 检查表单输入，没有发送或保存数据。

接下来要学习的是自己提供 API，并让项目和博客数据持久保存。
天气接口和联系表单不是本次作业要求的两类 CRUD 数据。
按文档列出的验收条件，先用 Swagger 完成后端验证；前端接入可在此后安排。

## 必须达到什么

| 部分 | 作业要求 |
| --- | --- |
| 技术 | .NET 10；一个 ASP.NET Core Web API 项目 |
| API 风格 | Controllers 或 Minimal API，选一种并能解释另一种 |
| 分工 | HTTP 处理、应用逻辑和 EF Core 数据访问可清楚区分 |
| 数据库 | SQL Server + EF Core + 自己的 DbContext；不可用 InMemory |
| 数据交换 | 所有输入和输出都通过 DTO，不直接接收或返回 EF 实体 |
| 功能 | Project 和 Blogpost 各五个 CRUD 接口，共十个 |
| 依赖注入 | 在 Program.cs 注册 DbContext 和 service/handler |
| 证据 | Swagger 截图，包括重启后的持久保存和删除后的 404 |
| 说明 | README、请求流程图、最多一页 A4 的架构论证 |
| 过程 | 至少六次 commit，分布在至少三周；反馈、来源、AI 使用及 LU3/LU4 工时 |

LU3 主要对应 rubric 1–5，LU4 对应 6–8；过程和口头解释的 criterion 9
对两个学习成果都适用。只展示接口能调用，不足以覆盖所有评分要求。

## 第一步今天做到哪里

**今天的验收目标：启动 .NET 10 项目，在浏览器打开 Swagger。**

已检测到本机的 .NET 10 SDK 和运行中的 SQL Server Express 服务。
起步项目位于 `backend/Portfolio.Api`，使用官方 `dotnet new webapi` 模板，
选择 controllers，并补上文档指定的 Swashbuckle 配置。

在仓库根目录运行：

```powershell
dotnet run --project backend/Portfolio.Api
```

看到终端提示 `Now listening on: http://localhost:5080` 后，打开：

```text
http://localhost:5080/swagger
```

目前没有业务接口，因此 Swagger 显示空接口列表是正常的。
用 `Ctrl+C` 停止。不要把这个阶段当成 CRUD 已完成。

然后打开 `Program.cs`，用自己的话解释这四件事：

1. `AddControllers()` 把 controller 所需的服务注册到容器。
2. `AddSwaggerGen()` 注册生成 API 文档所需的服务。
3. `UseSwagger()` 和 `UseSwaggerUI()` 提供文档和交互界面。
4. `MapControllers()` 把 controller 的动作接入请求路由。

理解后可以自己提交第一笔作业进度，例如 `Start opdracht 3 Web API project`。
只提交你检查过的内容，不要把 NuGet 缓存、bin、obj 或其他未完成的修改混入。

## 建议的结构

先考虑 **Controllers + Services + Data**，因为与 Les 5 的起点一致，
而且各层职责比较直观。这是待你确认和解释的建议，不是已经写好的最终论证。
Minimal API 也符合要求：它通过 MapGet/MapPost 等直接注册路由，
仍要把应用逻辑、DTO 转换和数据访问组织清楚。

计划中的请求流程是：

```text
Swagger UI
  -> ProjectsController：接收 GET /api/projects
  -> ProjectService：调用数据访问，并把实体转换成输出 DTO
  -> PortfolioDbContext：通过 EF Core 查询
  -> SQL Server：保存记录

查询结果 -> Service 的 DTO 转换 -> Controller 的 HTTP 响应 -> JSON
```

博客将采用同样的结构。Controllers、Services、Data、Models 和 DTOs
目前只是空目录；上面的流程还需要你实现，不能直接作为已完成代码的证据。

- **Entity / 实体**：描述数据库中保存的记录，包括数据库生成的 Id。
- **输入 DTO**：描述客户端可以提交的字段；创建时不让客户端选择 Id。
- **输出 DTO**：描述接口公开返回的字段。
- **DbContext**：让 EF Core 跟踪、查询和保存实体的入口。
- **依赖注入 / DI**：由框架提供已注册的 service 和 DbContext。

建议先用一个 monolith，也就是一个后端应用；是否合适以及理由，需要你自己
结合用户场景判断。不要把这些准备笔记当作 criterion 8 的最终一页 A4 答案。

## 下一步先完成 Project 的一条完整链路

先从 **GET /api/projects** 做起，让它真正穿过各层并返回 DTO：

1. 自己定义 `Project` 实体：Id、Titel、Beschrijving、Categorie、GitHubUrl、Datum。
2. 定义输出 DTO，想清楚客户端应该看到什么。
3. 添加 EF Core 10.x 的 SQL Server 和 Design 包，建立自己的 DbContext。
4. 配置本机 SQL Server 的连接字符串，通过 DI 注册 DbContext 和 service。
5. 用迁移建立数据库，先确认数据库连接成功。
6. 编写 service 的查询与映射，以及 controller 的 GET 动作。
7. 在 Swagger 执行 GET，检查 JSON 内容和 HTTP 200。没有记录时可以返回空数组。

之后添加 POST 的输入 DTO 和创建动作，先用 POST 插入记录再用 GET 查询。
这样就能证明 API 与数据库确实接通，再扩展其他操作。
不要直接把 `projecten.js` 数组换成另一份后端内存数组，作为最终存储方案。

## 十个接口清单

| 操作 | Project | Blogpost | 主要检查 |
| --- | --- | --- | --- |
| GET 全部 | `/api/projects` | `/api/blogposts` | 200，DTO 数组 |
| GET 单个 | `/api/projects/{id}` | `/api/blogposts/{id}` | 存在 200；不存在 404 |
| POST | `/api/projects` | `/api/blogposts` | 建议 201；请求体没有 Id |
| PUT | `/api/projects/{id}` | `/api/blogposts/{id}` | 更新成功；不存在 404 |
| DELETE | `/api/projects/{id}` | `/api/blogposts/{id}` | 删除成功；不存在 404 |

Blogpost 的字段至少是 Id、Titel、Inhoud、Publicatiedatum。
POST、PUT 需要输入 DTO，GET 需要输出 DTO。注意 POST 的响应也不能直接返回实体。
Controller 负责 HTTP 结果，业务逻辑放在 service，controller 不直接使用 DbContext。

## 后续顺序和证据

1. **起步阶段**：项目、Swagger、理解请求流程，完成真实的进度提交。
2. **Project 阶段**：数据库、DTO、DI、完整 CRUD，测试不存在的 Id。
3. **Blogpost 阶段**：按相同结构实现完整 CRUD，检查两类数据的一致性。
4. **验收阶段**：每一类分别完成 POST → GET 全部 → GET 单个 → PUT → GET
   → 停止并重启后 GET → DELETE → GET 404，全程保留 Swagger 截图。
5. **交付阶段**：整理实际代码的请求流程图；自己回答 criterion 8 的三个问题；
   更新 README，把 PDF 放进 Brightspace，把过程记录放进 Boulder。

至少六次 commit 应跟随实际进度分布在至少三周，不要集中制造历史记录。
可按“启动、Project 查询、Project CRUD、Blogpost CRUD、验收修正、交付材料”
组织提交，但实际日期要符合老师的截止时间。文档建议 week 7 完成并获取反馈，
week 8–9 处理反馈，最迟 week 9 重新评阅。

最终 PDF 还需要你的姓名、学号、班级、日期、老师可访问的仓库链接，
以及 Program.cs、两类 controller/service、DbContext、实体、输入/输出 DTO
和映射位置的代码证据。

criterion 8 的三道题应由你自己完成：职责如何组织；增加一个字段时哪些地方
要修改，EF Core 和 DTO 如何帮助；为什么选择 monolith 或 microservices。
至少一个回答要包含用户场景和 Microsoft Learn 来源。

## AI 使用和过程记录

作业原文将 AI 使用定位为 AIAS 2：规划、头脑风暴、解释、初步研究和结构。
它还说明 AI 代码补全应关闭，Visual Studio 模板或 scaffolding 可以使用，
但要记录并能解释。以上是作业的提交要求，供你据此安排自己的工作。

本次协助包括要求分析、环境检查、官方 CLI 模板创建、Swagger 起步配置和指南。
其中 Swagger 配置由 AI 辅助修改，应如实记录，不能描述为全部独立完成。
后续业务实现和最终架构论证由你自己理解、选择、编写和验证。
`opdracht-3-proces.md` 留有记录模板；工时、反馈和个人理解由你实际填写。

## 查阅资料

- [Microsoft Learn：Controller-based Web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0)
  ——参考路由和 controller 概念。该教程的 InMemory 示例不符合本次数据库要求。
- [Microsoft Learn：Swashbuckle 配置](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0)
  ——参考 AddSwaggerGen、UseSwagger、UseSwaggerUI；本作业要求在 .NET 10 手动使用该包。
- [Microsoft Learn：EF Core SQL Server provider](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)
- [Microsoft Learn：EF Core migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

课程 Les 5、Les 6 的 slides/demo 仍应作为主要课堂参考，当前未读取这些材料。
