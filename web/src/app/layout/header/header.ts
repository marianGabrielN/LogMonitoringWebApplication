import { Component } from "@angular/core";
import { MatToolbarModule } from "@angular/material/toolbar";

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [MatToolbarModule],
    templateUrl: './header.html',
    styleUrl: './header.scss',
})
export class HeaderComponent {}
