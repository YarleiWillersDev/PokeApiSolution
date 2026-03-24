# PokeApi - API Pokémon por Cor 🚀

## **Visão Geral**
API RESTful que consulta **Pokémon por cor** utilizando a PokeAPI externa, com **cache local SQLite** e resposta estruturada em JSON.

| **Informação** | **Detalhes** |
|----------------|--------------|
| **Base URL** | `https://localhost:5001/api/PokeApi` |
| **Versão** | `v1` |
| **Autenticação** | Não requerida |

## **Endpoints**

### **`GET /api/PokeApi/{colorName}/pokemons`**
**Consulta Pokémon de uma cor específica**

| Parâmetro | Tipo | Obrigatório | Descrição |
|-----------|------|-------------|-----------|
| `colorName` | `string` | ✅ Sim | Nome da cor em inglês (`red`, `blue`, `green`, etc.) |

#### **Exemplos**

**✅ Cor válida (`red`):**
```bash
GET /api/PokeApi/red/pokemons

```
#### ✅ Resposta 200 OK:
```bash
{
  "pokemonsNames": ["charizard", "ninetales", "lapras"],
  "color": "red"
}
```

#### ❌ Cor Inválida:
```bash
GET /api/PokeApi/vermelho/pokemons
````

#### ❌ Resposta 404 NOT FOUND:
```bash
{
  "statusCode": 404,
  "title": "Color Not Found",
  "message": "Cor 'vermelho' não existe na PokeAPI",
  "details": "Stack trace (dev mode)"
}
```


## 🛠 Tecnologias Usadas

| Tecnologia     | Versão | Propósito           |
| -------------- | ------ | ------------------- |
| C#             | -      | Linguagem principal |
| .NET 8         | -      | Framework           |
| EF Core        | 8.0.24 | ORM                 |
| EF Core SQLite | 8.0.24 | Banco local         |
| EF Core Design | 8.0.24 | Migrations          |
| AutoMapper     | 12.0.1 | Mapeamento DTOs     |
| Moq            | -      | Mocking testes      |
| Swagger        | -      | Documentação API    |

## 🧪 Testes
#### ✅ Cobertura completa com testes unitários e integração:
| Tipo       | Framework | Características                                                    |
| ---------- | --------- | ------------------------------------------------------------------ |
| Unitários  | MSTest    | Camada Service isolada                                             |
| Integração | xUnit     | Fluxo Controller → Service → DBMock PokeAPI (ambiente corporativo) |

Cenários testados:

- ✅ Cor válida → 200 OK + cache DB
- ✅ Cache segunda chamada (sem nova consulta API)
- ✅ Não duplicação de dados
- ✅ Cor inválida → 404 JSON custom
- ✅ Rota inexistente → 404

## 🚀 Como Executar
```bash
# Clone e execute
git clone <repo>
cd PokeApi
dotnet restore
dotnet ef database update
dotnet run

# Swagger UI
https://localhost:5001/swagger
```

## 📚 Fluxo da Aplicação
```bash
Cliente 
  ↓
Controller 
  ↓
Service 
  ↙         ↘
Cache DB  PokeAPI 
  ↓         ↓
Response JSON
  ↓
Exception Handler (404/500 custom JSON)
```

## 📈 Arquitetura
```bash
┌─────────────────┐      ┌──────────────────┐
│   Cliente       │ ───▶│   PokeApi API     │
│                 │      │  (externa)       │
└─────────────────┘      └──────────────────┘
                            ↓
┌─────────────────┐    ┌──────────────────┐
│ Controller      │◄───│   SQLite Cache   │
│                 │    │  (AppDbContext)  │
└─────────────────┘    └──────────────────┘
         ↓
┌─────────────────┐
│ Handler Custom  │ ←─ Exceções (400/404/500)
└─────────────────┘
```

