<div id="columnMain" style="margin-bottom: 0;padding-bottom: 0;">
    <div class="container">
        <div id="main-content">
            <p class="page-header">MAC Bank Funds Deposit/Withdraw</p>
            <form id="depwith_funds" name="depwith_funds" method="POST" action="index.php?action=depositwithdraw" enctype="multipart/form-data">
                <div id="display_details" class="row" >   
                    <div  class="col-md-12">
                        <div id="errorMessage" style="display: none;" class="alert alert-danger">
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
                        <div id="successMessage" style="display: none;" class="alert alert-success">
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
                </div>  <!-- end of row -->

                <div class="row">
                    <?php    
                    if(!empty($myAccounts)) {
			
                        echo  '<div class="col-sm-6" style="margin-bottom: 1rem;">
                                <label for="account_affected">Account:</label> 
                                <select id="account_affected" name="account_affected" class="form-control">';
                        if($selected_from_account == null) {    // if null then show 'select..' message
                            echo '<option value="" selected="selected">Select an Account</option>';
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

                    <?php    
			
                        echo  '<div class="col-sm-6">
                                <label for="transaction">Transaction:</label> 
                                <select id="transaction" name="transaction" class="form-control">';
                        if($selected_from_account == null) {    // if null then show 'select..' message
                            echo '<option value="" selected="selected">Select a Transaction</option>';
                        }
			            $selected = 'selected="selected"';
			            $doSelected = '';
			            if($selected_transaction != '' && $selected_transaction == 'deposit')
			                $doSelected = $selected;
			            echo "<option value='deposit' $doSelected>deposit</option>";
			            if($selected_transaction != '' && $selected_transaction == 'withdrawal')
			                $doSelected = $selected;
			            echo "<option value='withdraw' $doSelected>withdraw</option>";
                        echo '</select></div>';
           
	                ?>
                     
                </div><!-- end of row  -->
                <div class="cboth"></div>

                <div class="row" style="padding-top: 0.5rem;">
                    <div class="col-sm-6">
                        <label for="transfer_amount">Amount:</label> 
                        <input name="transfer_amount" id="transfer_amount" type="text" class="form-control" placeholder="Enter Amount" value="" />
                        <input style="margin-top: 1.75rem;" class="btn btn-warning" type="submit" value="Go" />
                    </div>
                    <div class="col-sm-6">
                        &nbsp;
                    </div>
                </div><!-- end of row  --> 

            </form>      
        </div>    
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#footer_txt").hide();
        $("#menu_transactions").addClass("selected");
    });
</script>
