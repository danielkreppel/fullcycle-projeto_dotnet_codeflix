﻿using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories
{
    public class ListCategoriesInput : PaginatedListInput, IRequest<ListCategoriesOutput>
    {
        public ListCategoriesInput() : base(page:1, perPage:15, search:"", sortBy:"", sortDir: SearchOrder.ASC)
        {
        }

        public ListCategoriesInput(int page = 1, int perPage = 15, string search = "", string sortBy = "", SearchOrder sortDir = SearchOrder.ASC)
            : base(page, perPage, search, sortBy, sortDir)
        {
        }
    }
}
