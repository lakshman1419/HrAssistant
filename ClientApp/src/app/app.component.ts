import { ChangeDetectionStrategy, Component } from '@angular/core';
import { HrAssistantComponent } from './components/hr-assistant/hr-assistant.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HrAssistantComponent],
  template: '<app-hr-assistant />',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {}
