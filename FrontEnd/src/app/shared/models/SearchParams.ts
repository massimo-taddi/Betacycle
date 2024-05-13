export class SearchParams {
  pageIndex: number = 1;
  pageSize: number = 20;
  search: string | null = null;
  sort: string = 'Desc';
  constructor() {}
}
