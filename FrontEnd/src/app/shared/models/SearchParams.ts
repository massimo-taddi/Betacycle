import { PaginatorParams } from './PaginatorParams';

export class SearchParams extends PaginatorParams {
  search: string | null = null;
  constructor() {
    super();
  }
}
