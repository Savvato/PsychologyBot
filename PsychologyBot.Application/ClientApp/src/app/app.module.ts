import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { WorkspaceComponent } from './workspace/workspace.component';
import { UsersListComponent } from './workspace/users-list/users-list.component';
import { ChatComponent } from './workspace/chat/chat.component';
import { DetailsComponent } from './workspace/details/details.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    WorkspaceComponent,
    UsersListComponent,
    ChatComponent,
    DetailsComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
