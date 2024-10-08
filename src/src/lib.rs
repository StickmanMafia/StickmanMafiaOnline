use solana_program::{                                                                                
    account_info::{next_account_info, AccountInfo},                                                  
    entrypoint,                                                                                      
    entrypoint::ProgramResult,                                                                       
    msg,                                                                                             
    program_error::ProgramError,                                                                     
    pubkey::Pubkey,                                                                                  
    system_instruction,                                                                              
    program::invoke,                                                                                 
};                                                                                                   
                                                                                                     
entrypoint!(process_instruction);                                                                    
                                                                                                     
fn process_instruction(                                                                              
    program_id: &Pubkey,                                                                             
    accounts: &[AccountInfo],                                                                        
    instruction_data: &[u8],                                                                         
) -> ProgramResult {                                                                                 
    let accounts_iter = &mut accounts.iter();                                                        
    let player1 = next_account_info(accounts_iter)?;                                                 
    let player2 = next_account_info(accounts_iter)?;                                                 
    let player3 = next_account_info(accounts_iter)?;                                                 
    let player4 = next_account_info(accounts_iter)?;                                                 
    let escrow = next_account_info(accounts_iter)?;                                                  
    let system_program = next_account_info(accounts_iter)?;                                          
                                                                                                     
    let amount = u64::from_le_bytes(instruction_data.try_into().map_err(|_|                          
ProgramError::InvalidInstructionData)?);                                                             
                                                                                                     
    // Transfer from each player to escrow                                                           
    for player in &[player1, player2, player3, player4] {                                            
        let transfer_instruction = system_instruction::transfer(                                     
            player.key,                                                                              
            escrow.key,                                                                              
            amount,                                                                                  
        );                                                                                           
        invoke(                                                                                      
            &transfer_instruction,                                                                   
            &[                                                                                       
                (**player).clone(),                                                                      
                escrow.clone(),                                                                      
                system_program.clone(),                                                              
            ],                                                                                       
        )?;                                                                                          
    }                                                                                                
                                                                                                     
    
               

    // INTEGRATION OF GAME'S API TO DETERMINE WINNER 

    // FOR NOW WE JUST HAVEN'T INTEGRATION OF OUR API

    let winner = player1;                           
                                                                                                     
    let total_amount = amount * 4;                              
    let transfer_to_winner = system_instruction::transfer(                                           
        escrow.key,                                                                                  
        winner.key,                                                                                  
        total_amount,                                                                                
    );                                                                                               
    invoke(                                                                                          
        &transfer_to_winner,                                                                         
        &[                                                                                           
            escrow.clone(),                                                                          
            winner.clone(),                                                                          
            system_program.clone(),                                                                  
        ],                                                                                           
    )?;                                                                                              
                                                                                                     
    msg!("Game completed successfully, winner paid");                                                
                                                                                                     
    Ok(())                                                                                           
}         