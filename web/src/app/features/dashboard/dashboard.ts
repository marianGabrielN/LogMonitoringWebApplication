import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { finalize } from "rxjs";
import { LogProcessingReport } from "../../core/models/interfaces/log-processing-report.interface";
import { LogService } from "../../core/services/log.service";
import { JobsSummaryComponent } from "./components/jobs-summary/jobs-summary";
import { JobsTableComponent } from "./components/jobs-table/jobs-table";
import { LogProcessorComponent } from "./components/log-processor/log-processor";

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        LogProcessorComponent,
        JobsSummaryComponent,
        JobsTableComponent,
    ],
    templateUrl: './dashboard.html',
    styleUrl: './dashboard.scss',
})
export class DashboardComponent {
    private logService = inject(LogService);

    isLoading = false;
    error: string | null = null;
    report: LogProcessingReport | null = null;

    onProcessLogs(): void {
        this.isLoading = true;
        this.error = null;
        this.report = null;

        this.logService
            .processLogs()
            .pipe(finalize(() => (this.isLoading = false)))
            .subscribe({
                next: (data) => {
                    this.report = data;
                },
                error: (err) => {
                    console.error(err);
                    this.error =
                        'Failed to process logs. Please ensure the API is running and the log file exists.';
                },
            });
    }
}
