import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/env.dev';
import { IReport } from '../Models/Report';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private url = environment.apiURL + '/Report';

  constructor(private _http: HttpClient) {}

  getReports() {
    return this._http.get<IReport[]>(this.url);
  }

  getReportById(id: number) {
    return this._http.get<IReport>(`${this.url}/${id}`);
  }

  addReport(report: IReport) {
    return this._http.post<IReport>(this.url, report);
  }

  editReport(id: number, report: IReport) {
    return this._http.patch<IReport>(`${this.url}/${id}`, report);
  }

  deleteReport(id: number) {
    return this._http.delete(`${this.url}/${id}`);
  }
}
