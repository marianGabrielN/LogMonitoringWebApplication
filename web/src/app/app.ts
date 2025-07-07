import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { HeaderComponent } from "./layout/header/header";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [
        RouterModule,
        HeaderComponent,
    ],
    templateUrl: './app.html',
})
export class App {}
