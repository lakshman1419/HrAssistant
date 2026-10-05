import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { HrAssistantComponent } from './hr-assistant.component';
import { AUTH_SESSION_STORAGE_KEY } from '../../services/auth.service';

describe('HrAssistantComponent', () => {
  let fixture: ComponentFixture<HrAssistantComponent>;
  let component: HrAssistantComponent;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HrAssistantComponent, HttpClientTestingModule],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(HrAssistantComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
    sessionStorage.clear();
  });

  it('adds a user question and displays the API answer', () => {
    component.questionControl.setValue('What is the leave policy?');
    component.sendMessage();

    const request = httpMock.expectOne('http://localhost:5118/api/hr/ask');
    expect(request.request.body).toBe('"What is the leave policy?"');
    request.flush({ answer: 'Please review the leave policy.' });

    expect(component.messages.at(-1)?.content).toBe('Please review the leave policy.');
    expect(component.isLoading).toBeFalse();
  });

  it('does not submit an empty question', () => {
    component.questionControl.setValue('   ');
    component.sendMessage();

    httpMock.expectNone('http://localhost:5118/api/hr/ask');
    expect(component.messages).toHaveSize(1);
  });

  it('shows a generic invalid-input message for HTTP 400 without exposing backend details', () => {
    component.questionControl.setValue('What is my leave balance?');
    component.sendMessage();

    httpMock.expectOne('http://localhost:5118/api/hr/ask').flush(
      { message: 'Sensitive backend validation detail' },
      { status: 400, statusText: 'Bad Request' }
    );

    expect(component.messages.at(-1)?.content).toBe(
      "I couldn't process that request. Please check your wording and try again."
    );
    expect(component.messages.at(-1)?.content).not.toContain('Sensitive backend validation detail');
    expect(component.isLoading).toBeFalse();

    component.questionControl.setValue('Try again');
    component.sendMessage();
    httpMock.expectOne('http://localhost:5118/api/hr/ask').flush(
      { message: 'Sensitive server detail' },
      { status: 500, statusText: 'Server Error' }
    );

    expect(component.messages.at(-1)?.content).toBe('The assistant is temporarily unavailable. Please try again.');
    expect(component.messages.at(-1)?.content).not.toBe(component.messages.at(-2)?.content);
    expect(component.messages.at(-1)?.content).not.toContain('Sensitive server detail');
  });

  it('clears the session and routes to login on logout', () => {
    const router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));
    sessionStorage.setItem(AUTH_SESSION_STORAGE_KEY, JSON.stringify({
      username: 'employee',
      displayName: 'Example Employee',
      role: 'Employee'
    }));

    component.logout();

    expect(sessionStorage.getItem(AUTH_SESSION_STORAGE_KEY)).toBeNull();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
