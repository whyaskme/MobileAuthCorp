<div id="columnMain" style="margin-bottom: 0;padding-bottom: 0;">
    <div class="container">
        <p class="page-header">MAC Bank - Pay Bills</p>
        <div class="row">
            <div class="col-sm-12">
                <div id="errorMessage"  style="display: none;" class="alert alert-danger">
                    <?php if(isset($errorArray) && !empty($errorArray)) {
                        echo '<script type="text/javascript">$("#errorMessage").show()</script>';
                        $counter = 0;
                        foreach($errorArray as $msg) {
                                if($counter > 0)
                                echo '<br />';
                                echo $msg;
                                $counter++;
                            }
                        }
                    ?>
                </div>
                <div id="successMessage"  style="display: none;" class="alert alert-success">
                    <?php if(isset($messageArray) && !empty($messageArray)) {
                        echo '<script type="text/javascript">$("#successMessage").show()</script>';
                        $counter = 0;
                        foreach($messageArray as $msg) {
                                if($counter > 0)
                                echo '<br />';
                                echo $msg;
                                $counter++;
                            }
                        }
                    ?>
                </div>
            </div>
        </div>    

        <form id="pay_bills" name="pay_bills" method="POST" action="index.php?action=paybills" enctype="multipart/form-data">        
            <div class="row">
                <div class="col-sm-6">
                    <?php    
                    if(!empty($myAccounts)) {
            
                        echo  '<div class=""><label>Account</label>
                                <select id="account_affected" name="account_affected" class="form-control">';
                        if($selected_from_account == null) {    // if null then show 'select..' message
                            echo '<option value="" selected="selected">Select Account</option>';
                        }
                        $selected = 'selected="selected"';
                        foreach($myAccounts as $account) {
                            $account = (array)$account;
			                $formattedNumber = substr($account['Number'], 0, 4).'-'.substr($account['Number'], 4, 4).'-'.substr($account['Number'], 8, 4).'-'.substr($account['Number'], 12, 4);  
                            $doSelected = '';
                            if($selected_account != '' && $selected_account == $account['Name'])
                                $doSelected = $selected;
                            echo "<option value='$account[Name]' $doSelected>$formattedNumber&nbsp;&nbsp;&nbsp;$account[Name] : $account[Balance]</option>";
                        }
                        echo '</select></div>';
                    }
           
                    ?>
                </div>
                <div class="col-sm-6">
                    <label>Amount</label>
                    <input type="text" class="form-control" name="pay_bill_amount" id="pay-bill-amount" value="" placeholder="Enter Amount Paid" />
                </div>  
            </div>  <!-- end of row  -->
            <div class="cboth"></div>
      
            <div class="row">
                <div class="col-md-12" style="margin: 1.75rem 0;">
                    <label>Select Bill to pay:</label>
                    <div style="margin:0 !important;padding:0 !important;border:1px solid #b3b3b3 !important;">
                        <table id="cart-content">
                            <thead>
                                <tr>
                                    <th>Invoice</th><th>Type</th><th>Name</th><th>Bill Date</th><th>Amount</th><th>Due Date</th><th>Status</th><th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                                <?php
                                    if(!empty($myBills)) {     
                                        $myBillsArray = $myBills;
                                        if(is_array($myBills) && $myBills[0] == null) {
                                            $myBillsArray = array();
                                            $myBillsArray[] = $myBills;
                                        }
   
                                        foreach($myBillsArray as $index => $bill) {
                                            $dueData = strtotime($bill['DueDate']);
                                            $today = time();
                                            if($today >= $dueData && save_money_amount($bill['AmountDue']) > 0) {   // determine which bills are 'due'
                                                $dueStatus = "<span class='error-msg-red text-bg-hilite-red pay-bill-button'>Due</span>";
                                            }
                                            else {
                                                $dueStatus = "<span class='msg-green'>Ok</span>";
                                            }
                                            if(save_money_amount($bill['AmountDue']) > 0) {
                                                $payStatus =  '<span class="msg-green text-bg-hilite-green pay-bill-button">Pay</span></td>';
                                            }
                                            else {
                                                    $payStatus = '<span class="msg-green">&nbsp;</span></td>';
                                            }                                         

                                            echo  '
                                <tr>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Invoice</strong></div>'.
                                    $bill['InvoiceNumber'].
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Type</strong></div>'.
                                    $bill['BusinessType'].
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Name</strong></div>'.
                                    $bill['Name'].
                                    '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Bill Date</strong></div>'. 
                                    $bill['BillingDate'].'</span>
                                    </td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Amount</strong></div><span class="cart-product-price bill-amount">'.
                                    $bill['AmountDue'].'</span>
                                    </td> 
	                                <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Due Date</strong></div>'.
	                                $bill['DueDate'].
	                                '</td>
                                    <td><div class="visible-sm visible-xs" style="margin-top:0.5rem;"><strong>Status</strong></div>'.
                                    $dueStatus.
    
	                                $payStatus.
	                                '   
                                </tr>';
                                    } 
                                }
                                else  {
                                    echo '<tr><td colspan="8">no bills...</td></tr>';
                                }
                                ?>
                            </tbody>
                        </table>
                    </div>
                </div>   <!-- end of col -->
            </div>  <!-- end of row -->
        </form>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#footer_txt").hide();
        $("#menu_payments").addClass("selected");
    });
</script>