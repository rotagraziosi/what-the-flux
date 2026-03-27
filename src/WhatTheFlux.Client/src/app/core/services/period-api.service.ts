import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Criterion,
  CreatePeriodRequest,
  PeriodDetail,
  PeriodSummary,
} from '../models/period.model';

@Injectable({ providedIn: 'root' })
export class PeriodApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  getPeriods(): Observable<PeriodSummary[]> {
    return this.http.get<PeriodSummary[]>(`${this.base}/periods`);
  }

  getPeriod(id: number): Observable<PeriodDetail> {
    return this.http.get<PeriodDetail>(`${this.base}/periods/${id}`);
  }

  createPeriod(req: CreatePeriodRequest): Observable<PeriodDetail> {
    return this.http.post<PeriodDetail>(`${this.base}/periods`, req);
  }

  deletePeriod(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/periods/${id}`);
  }

  addDay(periodId: number): Observable<PeriodDetail> {
    return this.http.post<PeriodDetail>(`${this.base}/periods/${periodId}/days`, {});
  }

  removeDay(periodId: number, dayNumber: number): Observable<PeriodDetail> {
    return this.http.delete<PeriodDetail>(
      `${this.base}/periods/${periodId}/days/${dayNumber}`
    );
  }

  setCount(
    periodId: number,
    dayNumber: number,
    criterionId: number,
    count: number
  ): Observable<void> {
    return this.http.put<void>(
      `${this.base}/periods/${periodId}/days/${dayNumber}/counts/${criterionId}`,
      { count }
    );
  }

  getCriteria(): Observable<Criterion[]> {
    return this.http.get<Criterion[]>(`${this.base}/criteria`);
  }
}
