import { CommonModule } from "@angular/common";
import { Component, Input, Output, EventEmitter } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
    selector: 'app-log-processor',
    standalone: true,
    imports: [
        CommonModule,
        MatCardModule,
        MatButtonModule,
        MatProgressSpinnerModule,
    ],
    templateUrl: './log-processor.html',
    styleUrl: './log-processor.scss',
})
export class LogProcessorComponent {
    @Input() isLoading = false;
    @Input() error: string | null = null;
    @Output() process = new EventEmitter<void>();

    onProcessClick(): void {
        this.process.emit();
    }
}
