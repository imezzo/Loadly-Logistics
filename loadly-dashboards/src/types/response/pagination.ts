export interface Pagination {
  page: number
  size: number
  pages: number
  items: number
}

export interface ApiPagination {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export function apiPaginationAdapter(data: ApiPagination): Pagination {
  return {
    page: data.page,
    size: data.pageSize,
    pages: data.totalPages,
    items: data.totalCount,
  }
}
