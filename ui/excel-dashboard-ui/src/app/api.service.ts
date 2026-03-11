import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardRowDto {
  id: string;
  category: string;
  amount: number;
  currency: string;
  date: string;
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface DashboardSummaryDto {
  items: DashboardRowDto[];
  amountByCategory: { [category: string]: number };
}

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000';

  upload(file: File, currency: string): Observable<void> {
    const form = new FormData();
    form.append('file', file);
    form.append('currency', currency);

    return this.http.post<void>(`${this.baseUrl}/api/excel-dashboard/upload`, form);
  }

  getRows(skipCount: number, maxResultCount: number): Observable<PagedResultDto<DashboardRowDto>> {
    const params = new HttpParams()
      .set('skipCount', skipCount.toString())
      .set('maxResultCount', maxResultCount.toString());

    return this.http.get<PagedResultDto<DashboardRowDto>>(
      `${this.baseUrl}/api/excel-dashboard/rows`,
      { params },
    );
  }

  getSummary(): Observable<DashboardSummaryDto> {
    return this.http.get<DashboardSummaryDto>(`${this.baseUrl}/api/excel-dashboard/summary`);
  }
}
