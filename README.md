# TurismoApp

Aplicação ASP.NET Core MVC para cadastro e listagem de Pontos Turísticos, usando Entity Framework Core e SQL Server.

## Pré‑Requisitos

- .NET SDK
  - Recomendado: `8.0`
  - O projeto está em `net10.0`. Se você não tiver .NET 10 instalado, altere para `net8.0` no `TurismoApp.csproj`.
  - Download: https://dotnet.microsoft.com/download
- Visual Studio Community 2022
  - Workload “ASP.NET and web development”
  - Download: https://visualstudio.microsoft.com/pt-br/vs/community/
- SQL Server
  - SQL Server Express 2019/2022 ou `(localdb)\MSSQLLocalDB`
  - Download: https://www.microsoft.com/pt-br/sql-server/sql-server-downloads
- SSMS (opcional)
  - Download: https://aka.ms/ssmsfullsetup

## Configuração de Banco

- Arquivo: `appsettings.json`
- Chave: `ConnectionStrings:DefaultConnection`

Exemplos de `Server`:

- Instância SQL Express padrão: `Server=.\SQLEXPRESS;`
- LocalDB (VS): `Server=(localdb)\MSSQLLocalDB;`
- Instância nomeada: `Server=DESKTOP123\SQL2019;`
- Remoto com porta: `Server=localhost,1433; User Id=sa; Password=SuaSenhaSegura;`

Exemplo base (Windows Auth):

```json
"DefaultConnection": "Server=.\SQLEXPRESS;Database=TurismoDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

Dicas:

- Para usuário/senha, use: `Server=...;Database=TurismoDB;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;`
- `TrustServerCertificate=True` evita problemas de certificado local.

## Como Rodar (CLI)

```json
cd "C:\SEU\CAMINHO\PARA\TurismoApp"
dotnet restore
dotnet build
dotnet run
```

- Endpoints: `https://localhost:7160/` e `http://localhost:5085/` (conforme `launchSettings.json`)
- Rotas principais:
  - `PontosTuristicos/Index`
  - `PontosTuristicos/Create`
- O banco é criado automaticamente na primeira execução (`EnsureCreated`).

## Como Rodar (Visual Studio Community)

- Abra `TurismoApp.sln`
- Defina `TurismoApp` como projeto de inicialização
- Ajuste `appsettings.json` com seu `Server`
- `F5` (Debug) ou `Ctrl+F5` (Start Without Debugging)
- Acesse `PontosTuristicos/Create` para cadastrar

## Assets (Logo)

- Coloque sua imagem em `wwwroot\images\logo.jpg`
- As views utilizam `~/images/logo.jpg`. Se for `.png`, ajuste o nome na view.

## Troubleshooting

- Falha de conexão:
  - Verifique `Server`, `Database`, `Trusted_Connection` vs `User Id`/`Password`
  - Teste a conexão via SSMS
- SDK não encontrado:
  - Ajuste `TargetFramework` para `net8.0` em `TurismoApp.csproj` se não tiver .NET 10.
- Estático não carrega:
  - O projeto usa `app.UseStaticFiles()`. Coloque arquivos sob `wwwroot`.

---

## Segunda Etapa do Teste — SQL (SQL Server)

Tabela: `PontosTuristicos (Id, Nome, Cidade, Estado, Descricao, DataInclusao)`

1. SELECT com `EXISTS` — nomes da cidade “TUPA”

```sql
SELECT Nome FROM PontosTuristicos WHERE Cidade = 'TUPA'
```

2. SELECT — nome do ponto e nome da cidade de todos os pontos turísticos

```sql
SELECT Nome, Cidade FROM PontosTuristicos
```

3. SELECT — todos nomes e códigos, ordenados por nome

```sql
SELECT Nome, Id FROM PontosTuristicos ORDER BY Nome
```

4. DELETE — excluir registros com Id entre 100 e 200

```sql
DELETE FROM PontosTuristicos WHERE Id BETWEEN 100 AND 200
```

5. UPDATE — alterar estado “PR” para “SP” em todos os registros

```sql
UPDATE PontosTuristicos SET Estado = 'SP' WHERE Estado = 'PR'
```

6. INSERT — registro exemplo

```sql
INSERT INTO PontosTuristicos (Nome, Cidade, Estado, Descricao, DataInclusao)
VALUES ('Ponto A', 'TUPA', 'PR', 'Descrição do Ponto A', GETDATE())
```

---

## Terceira Etapa do Teste — Trocar Pneu Furado

1. Reduzir a velocidade e encostar em local seguro, plano e afastado do fluxo.
2. Ligar o pisca-alerta.
3. Puxar o freio de mão.
4. Engatar o carro (1ª/re ou “P” no automático).
5. Colocar calços nas rodas opostas ao pneu furado (se houver).
6. Posicionar o triângulo a distância segura (≈30 m ou conforme a via).
7. Pegar estepe, macaco e chave de roda.
8. Retirar a calota, se houver.
9. Afrouxar levemente os parafusos com o carro no chão.
10. Posicionar o macaco no ponto correto de apoio.
11. Levantar o carro até o pneu ficar fora do chão.
12. Remover totalmente os parafusos.
13. Retirar a roda furada.
14. Colocar o estepe alinhando os furos.
15. Rosquear os parafusos com a mão.
16. Apertar parcialmente com a chave de roda.
17. Baixar o carro até o pneu encostar no chão.
18. Retirar o macaco.
19. Apertar definitivamente os parafusos.
20. Guardar roda e ferramentas.
21. Recolher o triângulo.
22. Desligar o pisca-alerta e seguir viagem.
