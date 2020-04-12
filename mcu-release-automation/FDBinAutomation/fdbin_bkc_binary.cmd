@echo off
@echo ************************************************************************************************************
@echo *                                                                                                           *
@echo * Usage : FDBinBkcBinary.cmd [BKC automation tool path] [Json file relative to the tool path                *
@echo *         For example : FDBinBkcBinary.cmd C:\Projects\FDBinAutomation  Json\whitleyrp_icx_bkc_image_automation_config.json  *
@echo *                                                                                                           *
@echo ************************************************************************************************************
@echo tool path = %1
@echo json = %2
@echo -- Install virtualenv package --
pip install virtualenv --proxy=http://proxy.jf.intel.com:911
@echo -- Active virtual environment and launch the tool --
cd FDBinenv27\Scripts
rem call activate & cd C:\Projects\FDBinAutomation & python C:\Projects\FDBinAutomation\bkc_image_automation.py -j C:\Projects\FDBinAutomation\Json\whitleyrp_icx_bkc_image_automation_config.json
call activate & cd  %1 & python  %1\bkc_image_automation.py -j  %1\%2
