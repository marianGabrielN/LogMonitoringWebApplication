import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LogProcessingReport } from '../models/interfaces/log-processing-report.interface';

@Injectable({
    providedIn: 'root'
})
export class LogService {
    private readonly apiUrl = 'https://localhost:7165/api/logs/process';

    constructor(private http: HttpClient) { }

    processLogs(): Observable<LogProcessingReport> {
        return this.http.post<LogProcessingReport>(this.apiUrl, {});
    }
}
