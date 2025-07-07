import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";
import { MatCardModule } from "@angular/material/card";
import { LogProcessingReport } from "../../../../core/models/interfaces/log-processing-report.interface";

@Component({
    selector: 'app-jobs-summary',
    standalone: true,
    imports: [CommonModule, MatCardModule],
    templateUrl: './jobs-summary.html',
    styleUrl: './jobs-summary.scss',
})
export class JobsSummaryComponent {
    @Input() report: LogProcessingReport | null = null;
}
