Feature: Demo to show web/mobile automation run in a single flow using CDT

@mytag
Scenario Outline: Emulate on mobile after web page access
	Given i navigate to <webpage1> site
	And i get the page title for webbrowser
	When I change the browser settings to start emulating a <device>
	Given i navigate to <webpage2> site
	And i get the page title for emulator
	Then i validate the page titles
	And I change the browser settings to end emulating a <device>

	Examples:
	| device      | webpage1 | webpage2   |
	| IPHONE11PRO | google   | google     |
	| SAMSUNGS20  | google   | duckduckgo |
