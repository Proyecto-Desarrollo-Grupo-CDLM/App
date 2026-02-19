import type { ApiMetricDto, ApiSummaryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiMetricService {
  apiName = 'Default';
  

  deleteOldMetrics = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/api-metric/old-metrics',
    },
    { apiName: this.apiName,...config });
  

  getFilteredList = (statusCode: number, endpoint: string, fromDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMetricDto[]>({
      method: 'GET',
      url: '/api/app/api-metric/filtered-list',
      params: { statusCode, endpoint, fromDate },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMetricDto[]>({
      method: 'GET',
      url: '/api/app/api-metric',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getSummary = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiSummaryDto>({
      method: 'GET',
      url: '/api/app/api-metric/summary',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
