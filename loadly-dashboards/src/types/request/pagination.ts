export interface Pagination {
  page?: number
  size?: number
}

export interface ApiPagination {
  page?: number
  pageSize?: number
}

export function apiPaginationAdapter(data: Pagination): ApiPagination {
  return {
    ...(data.page ? { page: data.page } : {}),
    ...(data.size ? { pageSize: data.size } : {}),
  }
}
