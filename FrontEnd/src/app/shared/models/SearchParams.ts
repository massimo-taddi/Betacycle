export class SearchParams {
  pageIndex: number = 1;
  pageSize: number = 20;
  search?: string = '';
  sort: string = 'Desc';
  constructor() {}
}
