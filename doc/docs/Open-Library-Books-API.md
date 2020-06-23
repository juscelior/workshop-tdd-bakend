# Open Library Books API

[Open Library](https://openlibrary.org/) é um projeto para listar para guardar em um endereço da web um registro para cada livro já publicado. É um objetivo elevado, mas alcançável.

Até o momento, já foram reunidos mais de 20 milhões de registros com uma variedade de grandes catálogos, além de contribuições únicas, com mais a caminho.

## API

A [Central do desenvolvedor](https://openlibrary.org/developers) possui uma parte para listar as APIs disponíveis.

- [Books](https://openlibrary.org/dev/docs/api/books)
- [Covers](https://openlibrary.org/dev/docs/api/covers)
- [Lists](https://openlibrary.org/dev/docs/api/lists)
- [Read](https://openlibrary.org/dev/docs/api/read)
- [Recent Changes](https://openlibrary.org/dev/docs/api/recentchanges)
- [Search](https://openlibrary.org/dev/docs/api/search)
- [Search inside](https://openlibrary.org/dev/docs/api/search_inside)
- [Subjects](https://openlibrary.org/dev/docs/api/subjects)

Para início de conversa, preciso informar que não vamos consumir todas essas APIs. Vamos apenas fazer uso de 2 APIs [Books](https://openlibrary.org/dev/docs/api/books) e [Search](https://openlibrary.org/dev/docs/api/search), respectivamente para realizar uma pesquisa por ISBN e outra para pesquisar pelo título.

```json http
{
  "method": "get",
  "url": "https://openlibrary.org/api/books?bibkeys=ISBN:0-13-429106-9&format=json&jscmd=data"
}
```

```json http
{
  "method": "get",
  "url": "http://openlibrary.org/search.json?q=Developer+Testing+Building"
}
```

Infelizmente existe um risco para a utilização da API de [Search](https://openlibrary.org/dev/docs/api/search), eles informaram que é uma API experimental e pode sofrer alteração no futuro. Precisamos garantir que o contrato do serviço que estamos consumido não mude, e se mudar precisamos capturar o mais cedo possível essa alteração.