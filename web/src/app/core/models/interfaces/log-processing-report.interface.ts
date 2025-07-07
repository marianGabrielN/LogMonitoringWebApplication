import { Job } from "./job.interface";

export interface LogProcessingReport {
    totalJobsProcessed: number;
    completed: number;
    warnings: number;
    errors: number;
    running: number;
    jobs: Job[];
}
