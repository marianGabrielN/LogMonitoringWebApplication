import { CommonModule } from "@angular/common";
import { Component, OnChanges, Input, SimpleChanges } from "@angular/core";
import { MatTableModule, MatTableDataSource } from "@angular/material/table";
import { JobStatus } from "../../../../core/models/enums/job-status.enum";
import { Job } from "../../../../core/models/interfaces/job.interface";

@Component({
    selector: 'app-jobs-table',
    standalone: true,
    imports: [CommonModule, MatTableModule],
    templateUrl: './jobs-table.html',
    styleUrl: './jobs-table.scss',
})
export class JobsTableComponent implements OnChanges {
    @Input() jobs: Job[] = [];

    dataSource = new MatTableDataSource<Job>();
    displayedColumns: string[] = ['processId', 'description', 'duration', 'status'];

    private jobStatusNames: { [key in JobStatus]: string } = {
        [JobStatus.Running]: 'Running',
        [JobStatus.Completed]: 'Completed',
        [JobStatus.Warning]: 'Warning',
        [JobStatus.Error]: 'Error',
    };

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['jobs']) {
            this.dataSource.data = this.jobs;
        }
    }

    // This method is used by the template, so it must be public.
    getStatusName(status: JobStatus): string {
        return this.jobStatusNames[status];
    }
}
