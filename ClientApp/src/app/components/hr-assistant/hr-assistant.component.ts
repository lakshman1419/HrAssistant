import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, ElementRef, ViewChild, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { ChatMessage } from '../../models/chat-message.model';
import type { AuthUserProfile } from '../../models/auth.model';
import { AuthService } from '../../services/auth.service';
import { HrAssistantService } from '../../services/hr-assistant.service';

@Component({
  selector: 'app-hr-assistant',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './hr-assistant.component.html',
  styleUrl: './hr-assistant.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HrAssistantComponent {
  @ViewChild('messageList') private messageList?: ElementRef<HTMLElement>;

  readonly questionControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.maxLength(1000)]
  });
  readonly suggestions = [
    'What is the leave policy?',
    'How many sick leaves do I have?',
    'What are the working hours?',
    'How can I apply for leave?'
  ];
  readonly messages: ChatMessage[] = [
    {
      role: 'assistant',
      content: "Hello! I'm your AI HR Assistant.\n\nI can help with leave policies, attendance, payroll, employee benefits, HR policies, and company procedures.\n\nHow can I help you today?",
      timestamp: new Date()
    }
  ];

  isLoading = false;
  readonly currentUser: AuthUserProfile | null;

  private readonly hrAssistantService = inject(HrAssistantService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly changeDetector = inject(ChangeDetectorRef);

  constructor() {
    this.currentUser = this.authService.currentUser;
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigate(['/login']);
  }

  sendMessage(): void {
    const question = this.questionControl.value.trim();

    if (!question || this.isLoading || this.questionControl.invalid) {
      return;
    }

    this.messages.push({ role: 'user', content: question, timestamp: new Date() });
    this.questionControl.reset('');
    this.isLoading = true;
    this.scrollToLatest();

    this.hrAssistantService.askQuestion(question).subscribe({
      next: ({ answer }) => {
        this.messages.push({
          role: 'assistant',
          content: answer,
          timestamp: new Date()
        });
        this.isLoading = false;
        this.changeDetector.markForCheck();
        this.scrollToLatest();
      },
      error: (error: HttpErrorResponse) => {
        const content = error.status === 400
          ? "I couldn't process that request. Please check your wording and try again."
          : 'The assistant is temporarily unavailable. Please try again.';
        this.messages.push({
          role: 'assistant',
          content,
          timestamp: new Date()
        });
        this.isLoading = false;
        this.changeDetector.markForCheck();
        this.scrollToLatest();
      }
    });
  }

  useSuggestion(question: string): void {
    this.questionControl.setValue(question);
    this.sendMessage();
  }

  handleInputKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  private scrollToLatest(): void {
    setTimeout(() => {
      const element = this.messageList?.nativeElement;
      if (element) {
        element.scrollTo({ top: element.scrollHeight, behavior: 'smooth' });
      }
    });
  }
}
