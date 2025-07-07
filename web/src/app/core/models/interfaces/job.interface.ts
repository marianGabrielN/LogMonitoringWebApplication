import { JobStatus } from "../enums/job-status.enum";

export interface Job {
    processId: number;
    description: string;
    startTime: string;
    endTime?: string;
    duration?: string;
    status: JobStatus;
}
